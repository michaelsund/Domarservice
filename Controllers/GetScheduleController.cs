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
    public class GetScheduleController : ControllerBase
    {
        private readonly DomarserviceContext _context;

        public GetScheduleController(DomarserviceContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var r = new DoTest(_context).GetSchedules(id);
            if (r.Count > 0) {
              return Ok(r);
            } else {
              return Problem("Could not find any schedule for this Referee.");
            }
        }
    }
}
