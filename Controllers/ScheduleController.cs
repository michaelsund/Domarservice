using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Domarservice.DAL;
using Domarservice.BLL;
using Domarservice.Helpers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Domarservice.Controllers
{
  [ApiController]
  [Authorize(Roles="RefereeUser,Admin")]
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

        // Check if the user has the correct ID compared to the RefereeId
        if (schedule != null && claimId == schedule.Referee.Id)
        {
          return Ok(schedule);
        }
        else
        {
          _logger.LogWarning($"The user with the claimName {claimName} tried to access a record not owned, ScheduleId {id} where RefereeId was {schedule.Referee.Id}, token had claimId {claimId}");
          return StatusCode(500, new { message = "Kunde inte hämta schemat." });
        }
      }
      catch (Exception)
      {
        return StatusCode(500, new { message = "Problem att hämta schemat."});
      }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        bool deleteResult = await _scheduleRepository.DeleteScheduleById(id);
        if (!deleteResult)
        {
          return StatusCode(StatusCodes.Status400BadRequest, "Schemaposten kunde inte tas bort.");
        }
        return Ok();
      }
      catch (Exception e)
      {
        return StatusCode(500);
      }
    }
  }
}
