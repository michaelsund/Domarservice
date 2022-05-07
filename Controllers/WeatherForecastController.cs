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
    public class WeatherForecastController : ControllerBase
    {
        private readonly DomarserviceContext _context;

        public WeatherForecastController(DomarserviceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public string Get()
        {
            // new DoTest(_context).RunTest1();
            // new DoTest(_context).RunTest2();
            // new DoTest(_context).RunTest3();
            // new DoTest(_context).RunTest4();
            // new DoTest(_context).RunTest5();
            new DoTest(_context).RunTest6();

            return "yes";            
        }
    }
}
