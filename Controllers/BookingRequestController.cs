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
            return Ok("Schedule booking request sent");
          }
        }
        return StatusCode(500, new { message = "There was a problem booking the schedule." });
      }
      catch (Exception e)
      {
        return StatusCode(500, new { message = "There was a error booking the schedule." });
      }
    }

    [Authorize(Roles = "RefereeUser,Admin")]
    [HttpPost]
    [Route("referee-awnser")]
    public async Task<IActionResult> AwnserCompanyRequest([FromBody] AwnserCompanyRequestBody request)
    {
      try
      {
        var claimId = User.Identity.GetUserClaimId();
        var bookingRequest = await _bookingRequestRepository.AwnserBookingRequestFromCompany(request, claimId);

        if (bookingRequest)
        {
          return Ok($"The request was awnsered with {request.Accepting}");
        }
        else
        {
          _logger.LogWarning($"Could not accept the request from the company.. Log more!");
          return StatusCode(500, new { message = "The request could not be awnsered." });
        }
      }
      catch (Exception)
      {
        return StatusCode(500, new { message = "Problem awnsering the request from the company." });
      }
    }
  }
}
