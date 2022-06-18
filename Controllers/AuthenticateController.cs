using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Domarservice.DAL;
using Domarservice.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace Domarservice.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class AuthenticateController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly ISendMailHelper _sendMailHelper;
    private readonly IRefereeRepository _refereeRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    public AuthenticateController(
      ILogger<AuthenticateController> logger,
      ISendMailHelper sendMailHelper,
      IRefereeRepository refereeRepository,
      UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager,
      IConfiguration configuration
    )
    {
      _logger = logger;
      _sendMailHelper = sendMailHelper;
      _refereeRepository = refereeRepository;
      _userManager = userManager;
      _roleManager = roleManager;
      _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Could not verify email",
          Data = null,
        });
      }

      if (user.EmailConfirmed)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = $"The email {email} is allready verified.",
          Data = null,
        });
      }

      var result = await _userManager.ConfirmEmailAsync(user, token);
      if (result.Succeeded)
      {
        return Ok(new ApiResponse { Success = true, Message = $"The email {email} was verified", Data = null });

      }
      return StatusCode(500, new ApiResponse { Success = false, Message = $"The email {email} could not be verified.", Data = null });
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        return StatusCode(500, new { message = "Something went wrong." });
      }

      var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      _sendMailHelper.Send($"This is the link to activate your account. https://{{server}}:{{port}}/authenticate/confirm-email?token={confirmationToken}&email={user.Email}");

      return Ok(new ApiResponse { Success = true, Message = $"A new verification email has been sent", Data = null });
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("send-reset-password")]
    public async Task<IActionResult> SendResetPassword(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        _logger.LogError($"Failed to find the requesting user account when sending reset password email, {email}, the account does not exist.");
        return StatusCode(500, new ApiResponse { Success = false, Message = $"There was an error sending the reset password email", Data = null });
      }

      var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

      _sendMailHelper.Send($"This is the link to your password reset. https://blabla?email={user.Email}&resetToken={passwordToken}");
      return Ok(new ApiResponse { Success = true, Message = $"A new password has been sent to your email", Data = null });

    }

    [AllowAnonymous]
    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordBody request)
    {
      var user = await _userManager.FindByEmailAsync(request.email);
      if (user == null)
      {
        return StatusCode(500, new { message = "Something went wrong." });
      }

      try
      {
        var resetSuccess = await _userManager.ResetPasswordAsync(user, request.passwordToken, request.newPassword);
        if (resetSuccess.Succeeded)
        {
          return Ok(new { message = "Password has been changed." });
        }
        else
        {
          return StatusCode(500, new { message = "Could not reset password.", errors = resetSuccess.Errors });
        }
      }
      catch (Exception)
      {
        return StatusCode(500, new { message = "Could not set password." });
      }
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
      var user = await _userManager.FindByNameAsync(model.Username);

      if (user.LockoutEnd > DateTime.UtcNow)
      {
        return Unauthorized(new ApiResponse
        {
          Success = false,
          Message = "You're account has been locked out for X minutes.",
          Data = null
        });
      }

      if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
      {
        if (_userManager.SupportsUserLockout && await _userManager.GetAccessFailedCountAsync(user) > 0)
        {
          await _userManager.ResetAccessFailedCountAsync(user);
        }

        if (user.EmailConfirmed)
        {
          var userRoles = await _userManager.GetRolesAsync(user);

          // Since a CompanyUser och RefereeUser only can have one Id of the two connected to the user, we set it as "Id" here.
          int? mappedId = null;
          if (user.RefereeId != null)
          {
            mappedId = user.RefereeId;
          }
          else if (user.CompanyId != null)
          {
            mappedId = user.CompanyId;
          }

          var authClaims = new List<Claim>
          {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("Id", mappedId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          };

          foreach (var userRole in userRoles)
          {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
          }

          var token = CreateToken(authClaims);
          var refreshToken = GenerateRefreshToken();
          SetRefreshTokenInCookie(refreshToken);

          _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

          user.RefreshToken = refreshToken;
          user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

          await _userManager.UpdateAsync(user);

          return StatusCode(200, new ApiResponse
          {
            Success = true,
            Message = "You are now logged in.",
            Data = new
            {
              Token = new JwtSecurityTokenHandler().WriteToken(token),
              Expiration = token.ValidTo
            }
          }
          );
        }
        else
        {
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "Please confirm your email before logging in.",
            Data = null
          });
        }

      }
      if (_userManager.SupportsUserLockout && await _userManager.GetLockoutEnabledAsync(user))
      {
        await _userManager.AccessFailedAsync(user);
      }
      _logger.LogWarning($"Failed login attempt with the username {model.Username} from IP {HttpContext.Connection.RemoteIpAddress.ToString()}");
      return Unauthorized(new ApiResponse
      {
        Success = false,
        Message = "Username or password is wrong.",
        Data = null,
      });
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
      var userExists = await _userManager.FindByNameAsync(model.Username);
      if (userExists != null)
      {
        _logger.LogInformation($"User creating failed, the username allready exists {model.Username}, tried with email: {model.Email}");
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "That name is allready taken.",
          Data = null
        });
      }

      ApplicationUser user = new()
      {
        Email = model.Email,
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = model.Username
      };
      var result = await _userManager.CreateAsync(user, model.Password);
      if (!result.Succeeded)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "User creation failed! Please check user details and try again.",
          Data = null
        });
      }

      var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

      return Ok(new ApiResponse
      {
        Success = true,
        Message = "User created successfully. Please verify your email.",
        Data = new { ConfirmationToken = confirmationToken }
      });
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
    {
      var userExists = await _userManager.FindByNameAsync(model.Username);
      if (userExists != null)
      {
        _logger.LogInformation($"Admin user creating failed, the username allready exists {model.Username}, tried with email: {model.Email}");
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "That name is allready taken.",
          Data = null
        });
      }

      ApplicationUser user = new()
      {
        Email = model.Email,
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = model.Username
      };
      var result = await _userManager.CreateAsync(user, model.Password);
      if (!result.Succeeded)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Admin user creation failed! Please check user details and try again.",
          Data = null
        });
      }

      if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
      // if (!await _roleManager.RoleExistsAsync(UserRoles.CompanyUser))
      //   await _roleManager.CreateAsync(new IdentityRole(UserRoles.CompanyUser));

      if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
      {
        await _userManager.AddToRoleAsync(user, UserRoles.Admin);
      }
      // if (await _roleManager.RoleExistsAsync(UserRoles.CompanyUser))
      // {
      //   await _userManager.AddToRoleAsync(user, UserRoles.CompanyUser);
      // }
      return Ok(new ApiResponse
      {
        Success = true,
        Message = "Admin user created successfully. Please verify your email.",
        Data = null
      });
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshToken request)
    {
      if (request.AccessToken is null)
      {
        return BadRequest(new ApiResponse
        {
          Success = false,
          Message = "Invalid client request",
          Data = null
        });
      }

      var tokens = new TokenModel
      {
        AccessToken = request.AccessToken,
        RefreshToken = Request.Cookies["refreshToken"]
      };

      var principal = new ClaimsPrincipal();
      try
      {
        principal = GetPrincipalFromExpiredToken(tokens.AccessToken);

      }
      catch (Exception)
      {
        _logger.LogError($"Invalid access token posted to refresh-token");
      }
      if (principal.Identity == null)
      {
        return BadRequest(new ApiResponse
        {
          Success = false,
          Message = "Invalid access token or refresh token",
          Data = null
        });
      }

      // #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
      // #pragma warning disable CS8602 // Dereference of a possibly null reference.
      string username = principal.Identity.Name;
      // #pragma warning restore CS8602 // Dereference of a possibly null reference.
      // #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

      var user = await _userManager.FindByNameAsync(username);

      if (user == null || user.RefreshToken != tokens.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
      {
        return BadRequest(new ApiResponse
        {
          Success = false,
          Message = "Invalid access token or refresh token",
          Data = null
        });
      }

      var newAccessToken = CreateToken(principal.Claims.ToList());
      var newRefreshToken = GenerateRefreshToken();

      user.RefreshToken = newRefreshToken;
      await _userManager.UpdateAsync(user);

      SetRefreshTokenInCookie(newRefreshToken);

      return Ok(new ApiResponse
      {
        Success = true,
        Message = "Token updated successfully.",
        Data = new
        {
          Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
          Expiration = newAccessToken.ValidTo
        }
      });
    }

    private JwtSecurityToken CreateToken(List<Claim> authClaims)
    {
      var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
      _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

      var token = new JwtSecurityToken(
          issuer: _configuration["JWT:ValidIssuer"],
          audience: _configuration["JWT:ValidAudience"],
          expires: DateTime.UtcNow.AddMinutes(tokenValidityInMinutes),
          claims: authClaims,
          signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
          );

      return token;
    }

    private string GenerateRefreshToken()
    {
      var randomNumber = new byte[32];
      using (var rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
      }
    }

    private void SetRefreshTokenInCookie(string refreshToken)
    {
      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Expires = DateTime.UtcNow.AddDays(10),
      };
      Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
        ValidateLifetime = false
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
      if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        throw new SecurityTokenException("Invalid token");

      return principal;
    }
  }
}
