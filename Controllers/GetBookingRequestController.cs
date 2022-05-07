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
    public class GetBookingRequestController : ControllerBase
    {
        private readonly DomarserviceContext _context;

        public GetBookingRequestController(DomarserviceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var r = new DoTest(_context).RunTest6();
            if (r != null) {
              return Ok(r);
            } else {
              return Problem("Could not find the BookingRequest.");
            }
        }
    }
}
