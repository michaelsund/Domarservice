using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Domarservice.DAL;
using Domarservice.BLL;
using Domarservice.Models;

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

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
      var schedule = _scheduleRepository.GetScheduleById(id);
      if (schedule == null) {
        return NotFound("Schemat för domaren hittades inte.");
      }
      return Ok(schedule);
    }
  }
}
