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

namespace Domarservice.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class ScheduleController : ControllerBase
  {
    private readonly IScheduleRepository _scheduleRepository;
    public ScheduleController(IScheduleRepository scheduleRepository)
    {
      _scheduleRepository = scheduleRepository;
    }

    [Authorize]
    // [Authorize(Roles="User")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
      try
      {
        var schedule = await _scheduleRepository.GetScheduleById(id);
        if (schedule == null)
        {
          return NotFound("Schemat för domaren hittades inte.");
        }
        return Ok(schedule);
      }
      catch (Exception e)
      {
        return StatusCode(500);
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
