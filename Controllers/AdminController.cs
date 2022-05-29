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
using Domarservice.BLL;

namespace Domarservice.Controllers
{
  [Authorize(Roles = "Admin")]
  [ApiController]
  [Route("[controller]")]
  public class AdminController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly ISendMailHelper _sendMailHelper;
    private readonly IAdministrationService _administrationService;
    private readonly IRefereeRepository _refereeRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    public AdminController(
      ILogger<AuthenticateController> logger,
      ISendMailHelper sendMailHelper,
      IAdministrationService administrationService,
      IRefereeRepository refereeRepository,
      UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager,
      IConfiguration configuration
    )
    {
      _logger = logger;
      _sendMailHelper = sendMailHelper;
      _administrationService = administrationService;
      _refereeRepository = refereeRepository;
      _userManager = userManager;
      _roleManager = roleManager;
      _configuration = configuration;
    }

    // if (!await _roleManager.RoleExistsAsync(UserRoles.RefereeUser))
    // {
    //   await _roleManager.CreateAsync(new IdentityRole(UserRoles.RefereeUser));
    // }
    // if (await _roleManager.RoleExistsAsync(UserRoles.RefereeUser))

    // {
    //   await _userManager.AddToRoleAsync(user, UserRoles.RefereeUser);
    // }

    [HttpPost]
    [Route("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] RoleBody request)
    {
      var result = await _administrationService.AssignUserToRole(request);
      if (result)
      {
        return Ok($"The user with email {request.Email} was added to the role {request.Role}.");
      }
      return StatusCode(500, $"Problem adding the user with email {request.Email} to the role {request.Role}.");
    }

    [HttpPost]
    [Route("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] RoleBody request)
    {
      var result = await _administrationService.RemoveUserRole(request);
      if (result)
      {
        return Ok($"The user with email {request.Email} was removed from the role.");
      }
      return StatusCode(500, $"Problem removing the user with email {request.Email}'s role.");
    }

    [HttpGet]
    [Route("get-role")]
    public async Task<IActionResult> GetRole(string email)
    {
      var result = await _administrationService.GetUserRoles(email);
      return Ok(result);
    }
  }
}
