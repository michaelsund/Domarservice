using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Domarservice.DAL;
using Domarservice.BLL;
using Domarservice.Models;

namespace Domarservice.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class RefereeController : ControllerBase
  {
    private readonly IRefereeRepository _refereeRepository;
    public RefereeController(IRefereeRepository refereeRepository)
    {
      _refereeRepository = refereeRepository;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
      try
      {
        var referee = await _refereeRepository.GetRefeereById(id);
        if (referee == null)
        {
          return NotFound("Domaren hittades inte.");
        }
        return Ok(referee);
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
        bool deleteResult = await _refereeRepository.DeleteRefereeById(id);
        if (!deleteResult)
        {
          return StatusCode(StatusCodes.Status400BadRequest, "Domaren kunde inte tas bort.");
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
