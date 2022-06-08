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
          return Ok(schedule);
        }
        else
        {
          _logger.LogWarning($"Unauthorized access by {claimName}, ScheduleId {id} where RefereeId was {schedule.Referee.Id}, token had Id {claimId}");
          return Unauthorized(new { message = "You are not authorized to view this schedule." });
        }
      }
      catch (Exception)
      {
        return StatusCode(500, new { message = "There was a problem fetching the schedule." });
      }
    }

    [HttpGet("referee/{id:int}")]
    public async Task<IActionResult> GetAllByRefereeId(int id)
    {
      try
      {
        List<SimpleScheduleDto> schedules = await _scheduleRepository.GetSchedulesByRefereeId(id);
        return Ok(schedules);
      }
      catch (Exception)
      {
        return StatusCode(500, new { message = "There was a problem fetching all schedules for the referee." });
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
          return Ok("Schedule created.");
        }
        return StatusCode(500, "Problem creating schedule");
      }
      catch (Exception)
      {
        return StatusCode(500, "Invalid request creating schedule");
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
            return StatusCode(500, new { message = "Could not delete schedule." });
          }
          _logger.LogInformation($"The schedule with id {id} was deleted by user {claimName}.");
          return Ok(new { message = "Schemat togs bort." });
        }
        else
        {
          _logger.LogWarning($"The user with the claimName {claimName} tried to delete schedule with id {id} without permission to do so.");
          return StatusCode(500, new { message = "Schedule could not be deleted." });
        }
      }
      catch (Exception)
      {
        return StatusCode(500, new { message = "Problem reading the schema for delete." });
      }
    }
  }
}
