using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Domarservice.DAL;
using Domarservice.BLL;

namespace Domarservice.Controllers
{
  [Authorize(Roles = "Admin")]
  [ApiController]
  [Route("[controller]")]
  public class SetupDataController : ControllerBase
  {
    private readonly DomarserviceContext _context;
    private readonly ILogger _logger;

    public SetupDataController(DomarserviceContext context, ILogger<SetupDataController> logger)
    {
      _context = context;
      _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      _logger.LogInformation("Seeding testdata to db in SetupDataController.");
      try
      {
        for (int i = 1; i < 10; i++)
        {
          System.Console.WriteLine("Starting new insert round...");
          new DoTest(_context).AddReferee("Kalle" + i, "Karlsson", i);
          new DoTest(_context).AddScheduleDate(i);
          new DoTest(_context).AddCompanyAndScheduleFirstReferee("Smygehuk " + i, i);
          new DoTest(_context).AddEventForCompany("Smygehuk matchen " + i, i);
          new DoTest(_context).AddRefereeForEvent("Jag dömmer gärna som huvuddomare! " + i, i);
          new DoTest(_context).RespondYes(i);
          new DoTest(_context).AddCounty(i);
        }
        return Ok("Data seeded!");
      }
      catch (System.Exception)
      {
          return StatusCode(500);
      }
    }
  }
}
