using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Domarservice.DAL;
using Domarservice.BLL;
using Domarservice.Helpers;

namespace Domarservice.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class SetupDataController : ControllerBase
  {
    private readonly DomarserviceContext _context;
     private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger _logger;

    public SetupDataController(
      DomarserviceContext context,
      ILogger<SetupDataController> logger,
       UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager
    )
    {
      _context = context;
      _logger = logger;
      _userManager = userManager;
      _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      _logger.LogInformation("Seeding testdata to db in SetupDataController.");
      try
      {
        // Create all roles first, Admin, CompanyUser, RefereeUser
        _logger.LogInformation("Creating roles.");
        await new DoTest(_context, _userManager, _roleManager).SetupRoles();
        await new DoTest(_context, _userManager, _roleManager).AddFirstAdminUser();
       

        for (int i = 1; i < 10; i++)
        {
          System.Console.WriteLine("Starting new insert round...");
          new DoTest(_context, _userManager, _roleManager).AddReferee("Kalle" + i, "Karlsson", i);
          new DoTest(_context, _userManager, _roleManager).AddCompany("Smygehuk " + i, i);
          new DoTest(_context, _userManager, _roleManager).AddScheduleDate(i);
          // new DoTest(_context, _userManager, _roleManager).AddCompanyAndScheduleFirstReferee("Smygehuk " + i, i);
          new DoTest(_context, _userManager, _roleManager).AddEventForCompany("Smygehuk matchen " + i, i);
          // new DoTest(_context, _userManager, _roleManager).AddRefereeForEvent("Jag dömmer gärna som huvuddomare! " + i, i);
          // new DoTest(_context, _userManager, _roleManager).RespondYes(i);
          new DoTest(_context, _userManager, _roleManager).AddCounty(i);
        }

        // Add testusers for company and referee
        await new DoTest(_context, _userManager, _roleManager).AddFirstCompanyUser();
        await new DoTest(_context, _userManager, _roleManager).AddFirstRefereeUser();
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = "Data seeded",
          Data = null,
        });
      }
      catch (Exception e)
      {
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = $"Error seeding database",
            Data = e,
          });
      }
    }
  }
}
