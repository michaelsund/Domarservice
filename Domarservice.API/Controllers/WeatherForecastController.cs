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
            // var data = _context.Companies.ToList();
            new CreateAllAndTest(_context).RunCreate();
            
            return "yes";
        }
    }
}
