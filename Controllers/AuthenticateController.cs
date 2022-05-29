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
using Domarservice.Models;
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
        return StatusCode(500, new { message = "Could not verify email." });
      }

      if (user.EmailConfirmed)
      {
        return StatusCode(500, new { result = false, message = $"The email {email} is allready verified." });
      }

      var result = await _userManager.ConfirmEmailAsync(user, token);
      if (result.Succeeded)
      {
        return Ok(new { result = result.Succeeded, message = $"The email {email} was verified" });
      }
      return StatusCode(500, new { result = result.Succeeded, message = $"The email {email} could not be verified." });
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

      return Ok(new {
        message = "A new verification email has been sent."
      });
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("send-reset-password")]
    public async Task<IActionResult> SendResetPassword(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        return StatusCode(500, new { message = "Something went wrong." });
      }

      var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

      _sendMailHelper.Send($"This is the link to your password reset. https://blabla?email={user.Email}&resetToken={passwordToken}");
      return Ok(new {
        message = "A new password email has been sent."
      });
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
      if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
      {
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

          return Ok(new
          {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo
          });
        }
        else
        {
          return StatusCode(500, new { message = "Please confirm your email before logging in."});
        }

      }
      return Unauthorized();
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
      var userExists = await _userManager.FindByNameAsync(model.Username);
      if (userExists != null)
        return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "User already exists!" });

      ApplicationUser user = new()
      {
        Email = model.Email,
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = model.Username
      };
      var result = await _userManager.CreateAsync(user, model.Password);
      if (!result.Succeeded)
        return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "User creation failed! Please check user details and try again." });

      var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

      return Ok(new {
        success = true,
        message = "User created successfully. Please verify your email.",
        confirmationToken = confirmationToken
      });
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
    {
      var userExists = await _userManager.FindByNameAsync(model.Username);
      if (userExists != null)
        return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "User already exists!" });

      ApplicationUser user = new()
      {
        Email = model.Email,
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = model.Username
      };
      var result = await _userManager.CreateAsync(user, model.Password);
      if (!result.Succeeded)
        return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "User creation failed! Please check user details and try again." });

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
      return Ok(new { success = true, message = "User created successfully!" });
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshToken request)
    {
      if (request.AccessToken is null)
      {
        return BadRequest("Invalid client request");
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
      catch (System.Exception)
      {
        _logger.LogError($"Invalid access token posted to refresh-token");
      }
      if (principal.Identity == null)
      {
        return BadRequest("Invalid access token or refresh token");
      }

      // #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
      // #pragma warning disable CS8602 // Dereference of a possibly null reference.
      string username = principal.Identity.Name;
      // #pragma warning restore CS8602 // Dereference of a possibly null reference.
      // #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

      var user = await _userManager.FindByNameAsync(username);

      if (user == null || user.RefreshToken != tokens.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
      {
        return BadRequest("Invalid access token or refresh token");
      }

      var newAccessToken = CreateToken(principal.Claims.ToList());
      var newRefreshToken = GenerateRefreshToken();

      user.RefreshToken = newRefreshToken;
      await _userManager.UpdateAsync(user);

      SetRefreshTokenInCookie(newRefreshToken);

      return Ok(new
      {
        Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
        Expiration = newAccessToken.ValidTo
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
