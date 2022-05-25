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
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Domarservice.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AuthenticateController : ControllerBase
  {
    private readonly IRefereeRepository _refereeRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    public AuthenticateController(
      IRefereeRepository refereeRepository,
      UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager,
      IConfiguration configuration
    )
    {
      _refereeRepository = refereeRepository;
      _userManager = userManager;
      _roleManager = roleManager;
      _configuration = configuration;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
      var user = await _userManager.FindByNameAsync(model.Username);
      if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
      {
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
          new Claim("Name", user.UserName),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var userRole in userRoles)
        {
          authClaims.Add(new Claim("Role", userRole));
        }

        var token = CreateToken(authClaims);
        var refreshToken = GenerateRefreshToken();

        _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

        await _userManager.UpdateAsync(user);

        return Ok(new
        {
          Token = new JwtSecurityTokenHandler().WriteToken(token),
          RefreshToken = refreshToken,
          Expiration = token.ValidTo
        });
      }
      return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
      var userExists = await _userManager.FindByNameAsync(model.Username);
      if (userExists != null)
        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

      ApplicationUser user = new()
      {
        Email = model.Email,
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = model.Username
      };
      var result = await _userManager.CreateAsync(user, model.Password);
      if (!result.Succeeded)
        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

      return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
    {
      var userExists = await _userManager.FindByNameAsync(model.Username);
      if (userExists != null)
        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

      ApplicationUser user = new()
      {
        Email = model.Email,
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = model.Username
      };
      var result = await _userManager.CreateAsync(user, model.Password);
      if (!result.Succeeded)
        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

      if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
      if (!await _roleManager.RoleExistsAsync(UserRoles.User))
        await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

      if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
      {
        await _userManager.AddToRoleAsync(user, UserRoles.Admin);
      }
      if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
      {
        await _userManager.AddToRoleAsync(user, UserRoles.User);
      }
      return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
      if (tokenModel is null)
      {
        return BadRequest("Invalid client request");
      }

      string? accessToken = tokenModel.AccessToken;
      string? refreshToken = tokenModel.RefreshToken;

      var principal = GetPrincipalFromExpiredToken(accessToken);
      if (principal == null)
      {
        return BadRequest("Invalid access token or refresh token");
      }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
      string username = principal.Identity.Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

      var user = await _userManager.FindByNameAsync(username);

      if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
      {
        return BadRequest("Invalid access token or refresh token");
      }

      var newAccessToken = CreateToken(principal.Claims.ToList());
      var newRefreshToken = GenerateRefreshToken();

      user.RefreshToken = newRefreshToken;
      await _userManager.UpdateAsync(user);

      return new ObjectResult(new
      {
        accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
        refreshToken = newRefreshToken
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
