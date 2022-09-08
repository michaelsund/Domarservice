using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Domarservice.DAL;
using Domarservice.Helpers;
using Domarservice.Models;
using Microsoft.AspNetCore.Authorization;

namespace Domarservice.Controllers
{
  [ApiController]
  [Authorize]
  [Route("[controller]")]
  public class RefereeController : ControllerBase
  {
    private readonly IRefereeRepository _refereeRepository;
    private readonly ILogger _logger;

    public RefereeController(IRefereeRepository refereeRepository, ILogger<RefereeController> logger)
    {
      _refereeRepository = refereeRepository;
      _logger = logger;
    }

    [Authorize(Roles = "RefereeUser,CompanyUser,Admin")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
      try
      {
        var referee = await _refereeRepository.GetRefeereById(id);
        if (referee == null)
        {
          _logger.LogError($"RefereeRepo GetRefereeById returned null, could be a problem between user RefereeId: {id} mapping to Referee object, probably a mapping doesnt exist!");
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "Could not find the referee.",
            Data = null
          });
        }
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = "Referee found.",
          Data = referee
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "There was an error finding the referee",
          Data = null,
        });
      }
    }

    // This endpoint should never be called except in rare cases by admin.
    // Even if a user connected to this referee object is deleted, the referee should still exist.
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        var referee = await _refereeRepository.GetRefeereById(id);
        var claimId = User.Identity.GetUserClaimId();
        var claimName = User.Identity.GetUserClaimName();
        var isAdmin = User.Identity.CheckAdminRole();
        
        if ((referee != null && claimId == referee.Id) || (referee != null && isAdmin))
        {
          bool deleteResult = await _refereeRepository.DeleteRefereeById(id);
          if (!deleteResult)
          {
            return StatusCode(500, new ApiResponse
            {
              Success = false,
              Message = "The referee could not be deleted",
              Data = null,
            });
          }
          _logger.LogWarning($"The user {User.Identity.Name} deleted the referee {referee.Surname} {referee.Lastname} with id: {id}");
          return StatusCode(200, new ApiResponse
          {
            Success = true,
            Message = "The referee was deleted",
            Data = null,
          });
        }

        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "You dont have permission to delete the referee",
          Data = null,
        });

      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "There was a problem deleting the referee",
          Data = null,
        });
      }
    }
  }
}
