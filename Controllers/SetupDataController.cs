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
    public class SetupDataController : ControllerBase
    {
        private readonly DomarserviceContext _context;

        public SetupDataController(DomarserviceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public string Get()
        {
            // Create 99 Referees with one schedule item, create a company that reserves that slot.
            for (int i = 1; i < 10; i++)
            {
                System.Console.WriteLine("Starting new insert round...");
                new DoTest(_context).AddReferee("Kalle " + i, "Karlsson");
                new DoTest(_context).AddScheduleDate(i);
                new DoTest(_context).AddCompanyAndScheduleFirstReferee("Smygehuk " + i, i);
                new DoTest(_context).RespondYes(i);
                new DoTest(_context).AddCounty(i);
            }    

            return "yes";            
        }
    }
}
