using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Domarservice.DAL;
using Domarservice.BLL;

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
    public Referee Get(int id)
    {
      return _refereeRepository.GetRefeereById(id);
    }
  }
}
