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
  public class GetRefereeController : ControllerBase
  {
    private readonly IRefereeRepository _refereeRepository;
    public GetRefereeController(IRefereeRepository refereRepository)
    {
      _refereeRepository = refereRepository;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
      var referee = _refereeRepository.GetRefeereById(id);
      if (referee == null) {
        return NotFound("Domaren hittades inte.");
      }
      return Ok(referee);
    }
  }
}
