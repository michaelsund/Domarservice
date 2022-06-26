using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Domarservice.DAL;
using Domarservice.Helpers;
using Domarservice.Models;

namespace Domarservice.Controllers
{
  [ApiController]
  [Authorize]
  [Route("[controller]")]
  public class ScheduleController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly IScheduleRepository _scheduleRepository;
    public ScheduleController(ILogger<ScheduleController> logger, IScheduleRepository scheduleRepository)
    {
      _logger = logger;
      _scheduleRepository = scheduleRepository;
    }

    [Authorize(Roles = "RefereeUser,CompanyUser,Admin")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
      try
      {
        var schedule = await _scheduleRepository.GetScheduleById(id);
        var claimId = User.Identity.GetUserClaimId();
        var claimName = User.Identity.GetUserClaimName();
        var isAdmin = User.Identity.CheckAdminRole();

        // Check if the user has the correct ID compared to the RefereeId
        if ((schedule != null && claimId == schedule.Referee.Id) || (schedule != null && isAdmin))
        {
          return StatusCode(200, new ApiResponse
          {
            Success = true,
            Message = "Schedule found",
            Data = schedule,
          });
        }
        else
        {
          _logger.LogWarning($"Unauthorized access by {claimName}, ScheduleId {id} where RefereeId was {schedule.Referee.Id}, token had Id {claimId}");
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "You are not authorized to view this schedule",
            Data = null,
          });
        }
      }
      catch (Exception)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "There was a problem fetching the schedule",
          Data = null,
        });
      }
    }

    [Authorize(Roles = "RefereeUser,CompanyUser,Admin")]
    [HttpGet("referee/{id:int}")]
    public async Task<IActionResult> GetAllByRefereeId(int id)
    {
      try
      {
        List<SimpleScheduleDto> schedules = await _scheduleRepository.GetSchedulesByRefereeId(id);
        if (schedules != null)
        {
          return StatusCode(200, new ApiResponse
          {
            Success = true,
            Message = $"Success fetching schedules for referee with id {id}",
            Data = schedules
          });
        }
        return StatusCode(500, new ApiResponse {
          Success = true,
          Message = $"No schedules found for referee with id {id}",
          Data = null
        });
      }
      catch (Exception)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "There was a problem fetching all schedules for the referee.",
          Data = null
        });
      }
    }

    [Authorize(Roles = "RefereeUser,Admin")]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromBody] CreateScheduleBody request)
    {
      try
      {
        var claimId = User.Identity.GetUserClaimId();
        var claimName = User.Identity.GetUserClaimName();
        var result = await _scheduleRepository.CreateSchedule(claimId, request.AvailAbleAt);
        if (result)
        {
          _logger.LogInformation($"Schedule created for {claimName} with refereeId {claimId} at {request.AvailAbleAt}");
          return StatusCode(200, new ApiResponse
          {
            Success = true,
            Message = "Schedule created",
            Data = null,
          });
        }
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Problem creating schedule",
          Data = null,
        });
      }
      catch (Exception)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Invalid request creating schedule",
          Data = null,
        });
      }
    }

    [Authorize(Roles = "RefereeUser,Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        var schedule = await _scheduleRepository.GetScheduleById(id);
        var claimId = User.Identity.GetUserClaimId();
        var claimName = User.Identity.GetUserClaimName();
        var isAdmin = User.Identity.CheckAdminRole();

        if ((schedule != null && claimId == schedule.Referee.Id) || (schedule != null && isAdmin))
        {
          try
          {
            await _scheduleRepository.DeleteScheduleById(id);
          }
          catch (Exception)
          {
            return StatusCode(500, new ApiResponse
            {
              Success = false,
              Message = "Error deleting schedule",
              Data = null,
            });
          }
          _logger.LogInformation($"The schedule with id {id} was deleted by user {claimName}.");
          return StatusCode(200, new ApiResponse
          {
            Success = true,
            Message = "Schedule was deleted",
            Data = null,
          });
        }
        else
        {
          _logger.LogWarning($"The user with the claimName {claimName} tried to delete schedule with id {id} without permission to do so.");
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "Schedule could not be deleted, you dont have permission.",
            Data = null,
          });
        }
      }
      catch (Exception)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Problem the schedule for deletion",
          Data = null,
        });
      }
    }
  }
}
