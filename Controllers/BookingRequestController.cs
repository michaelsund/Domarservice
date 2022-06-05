using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Domarservice.DAL;
using Domarservice.Helpers;
using Domarservice.Models;

namespace Domarservice.Controllers
{
  [ApiController]
  [Authorize]
  [Route("[controller]")]
  public class BookingRequestController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly IBookingRequestRepository _bookingRequestRepository;
    public BookingRequestController(ILogger<ScheduleController> logger, IBookingRequestRepository bookingRequestRepository)
    {
      _logger = logger;
      _bookingRequestRepository = bookingRequestRepository;
    }

    [Authorize(Roles = "CompanyUser,Admin")]
    [HttpPost]
    [Route("request-by-company")]
    public async Task<IActionResult> BookingRequestByCompany([FromBody] BookScheduleByCompanyBody request)
    {
      try
      {
        var claimId = User.Identity.GetUserClaimId();
        if (claimId > 0)
        {
          request.CompanyId = claimId;
          var result = await _bookingRequestRepository.AddBookingRequestByCompany(request);
          if (result)
          {
            return Ok("Schedule is now booked");
          }
        }
        return StatusCode(500, new { message = "There was a problem booking the schedule." });
      }
      catch (Exception e)
      {
        return StatusCode(500, new { message = "There was a error booking the schedule." });
      }
    }
  }
}
