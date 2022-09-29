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
    public BookingRequestController(ILogger<BookingRequestController> logger, IBookingRequestRepository bookingRequestRepository)
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
          var result = await _bookingRequestRepository.AddBookingRequestByCompany(request, claimId);
          if (result)
          {
            return StatusCode(200, new ApiResponse
            {
              Success = true,
              Message = "Schedule booking request sent",
              Data = null,
            });
          }
        }
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "There was a problem booking the schedule",
          Data = null,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "There was a error booking the schedule",
          Data = null,
        });
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
          return StatusCode(200, new ApiResponse
          {
            Success = true,
            Message = $"The request was awnsered with {request.Accepted}",
            Data = null,
          });
        }
        else
        {
          _logger.LogWarning($"Could not accept the request from the company.. Log more!");
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "The request could not be awnsered",
            Data = null,
          });
        }
      }
      catch (Exception)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Problem awnsering the request from the company.",
          Data = null,
        });
      }
    }

    [Authorize(Roles = "RefereeUser,Admin")]
    [HttpPost]
    [Route("request-by-referee")]
    public async Task<IActionResult> BookingRequestByRefereeOnCompanyEvent([FromBody] BookCompanyEventByRefereeBody request)
    {
      try
      {
        var claimId = User.Identity.GetUserClaimId();
        if (claimId > 0)
        {
          ResultWithMessage result = await _bookingRequestRepository.AddBookingRequestByReferee(request, claimId);

          // Sets better errormessage from repository and helper.
          return StatusCode(result.Result ? 200 : 500, result);
        }
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Ett fel uppstod när anmälan skickas.",
          Data = null,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Ett problem uppstod när anmälan skulle skickas.",
          Data = null,
        });
      }
    }

    [Authorize(Roles = "RefereeUser,Admin")]
    [HttpGet("revoke-request-by-referee/{requestId:int}")]
    public async Task<IActionResult> RevokeBookingRequestByRefereeOnCompanyEvent(int requestId)
    {
      try
      {
        var claimId = User.Identity.GetUserClaimId();
        if (claimId > 0)
        {
          var result = await _bookingRequestRepository.RemoveBookingRequestByReferee(requestId, claimId);
          return StatusCode(result.Result ? 200 : 500, new ApiResponse
            {
              Success = result.Result,
              Message = result.Message,
              Data = result.Data,
            });
        }
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Ett fel uppstod när avbokningen skickas.",
          Data = null,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Ett problem uppstod när avbokningen skulle skickas.",
          Data = null,
        });
      }
    }

    [Authorize(Roles = "CompanyUser,Admin")]
    [HttpPost]
    [Route("company-awnser")]
    public async Task<IActionResult> AwnserRefereeRequest([FromBody] AwnserRefereeRequestBody request)
    {
      try
      {
        var claimId = User.Identity.GetUserClaimId();
        var bookingRequest = await _bookingRequestRepository.AwnserBookingRequestFromReferee(request, claimId);

        if (bookingRequest)
        {
          return StatusCode(200, new ApiResponse
          {
            Success = true,
            Message = $"The request was awnsered with {request.Accepted}",
            Data = null,
          });
        }
        else
        {
          _logger.LogWarning($"Could not accept the request from the referee.. Log more!");
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "The request could not be awnsered.",
            Data = null,
          });
        }
      }
      catch (Exception)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Problem awnsering the request from the referee.",
          Data = null,
        });
      }
    }

  }
}
