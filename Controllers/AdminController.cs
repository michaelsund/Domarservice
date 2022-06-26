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

    [HttpPost]
    [Route("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] RoleBody request)
    {
      var result = await _administrationService.AssignUserToRole(request);
      if (result)
      {
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = $"The user with email {request.Email} was added to the role {request.Role}.",
          Data = null,
        });
      }
      return StatusCode(500, new ApiResponse
      {
        Success = false,
        Message = $"Problem adding the user with email {request.Email} to the role {request.Role}.",
        Data = null,
      });
    }

    [HttpDelete]
    [Route("delete-user")]
    public async Task<IActionResult> DeleteUser(string email)
    {
      var result = await _administrationService.DeleteUser(email);
      if (result)
      {
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = $"The user with email {email} was deleted.",
          Data = null,
        });
      }
      return StatusCode(500, new ApiResponse
      {
        Success = false,
        Message = $"Problem deleting the user with email: {email}",
        Data = null,
      });
    }

    // The "delete me" endpoint, usable by admins to remove all info on a user that's a referee.
    [HttpDelete]
    [Route("delete-user-referee")]
    public async Task<IActionResult> DeleteUserAndReferee(string email)
    {
      var result = await _administrationService.DeleteUserAndReferee(email);
      if (result)
      {
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = $"The user with email {email} was deleted both as user and referee.",
          Data = null,
        });
      }
      return StatusCode(500, new ApiResponse
      {
        Success = false,
        Message = $"Problem deleting the user and referee with email: {email}",
        Data = null,
      });
    }

    [HttpPost]
    [Route("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] RoleBody request)
    {
      try
      {
        var result = await _administrationService.RemoveUserRole(request);
        if (result)
        {
          return StatusCode(200, new ApiResponse
          {
            Success = true,
            Message = $"The user with email {request.Email} was removed from the role.",
            Data = null,
          });
        }
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = $"Problem removing the user with email {request.Email}'s role.",
          Data = null,
        });
      }
      catch (Exception)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = $"Error removing the user with email {request.Email}'s role.",
          Data = null,
        });
      }
    }

    [HttpGet]
    [Route("get-role")]
    public async Task<IActionResult> GetRole(string email)
    {
      try
      {
        var result = await _administrationService.GetUserRoles(email);
        if (result.Count > 0)
        {
          return Ok(new ApiResponse
          {
            Success = true,
            Message = $"The current role for the user {email}.",
            Data = result,
          });
        }
        return Ok(new ApiResponse
        {
          Success = true,
          Message = $"There is no roles assigned to the user {email}.",
          Data = null,
        });
      }
      catch (Exception e)
      {
        _logger.LogError($"Problem getting roles for user with email {email} with the exception: {e}");
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = $"There was an error trying to get the roles for {email}",
          Data = null,
        });
      }
    }
  }
}
