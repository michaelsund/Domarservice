using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Domarservice.DAL;
using Domarservice.Helpers;
using Domarservice.Models;

namespace Domarservice.Controllers
{
  [ApiController]
  [Authorize]
  [Route("[controller]")]
  public class CompanyEventController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly ICompanyEventRepository _companyEventRepository;
    public CompanyEventController(ILogger<CompanyEventController> logger, ICompanyEventRepository companyEventRepository)
    {
      _logger = logger;
      _companyEventRepository = companyEventRepository;
    }

    [Authorize(Roles = "RefereeUser,CompanyUser,Admin")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
      try
      {
        var companyEvent = await _companyEventRepository.GetCompanyEventById(id);
        if (companyEvent == null)
        {
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "Kunde inte hitta matchen.",
            Data = null,
          });
        }
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = "Här är matchen.",
          Data = companyEvent,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Ett problem uppstod när matchen skulle hämtas.",
          Data = null,
        });
      }
    }

    // Used for the indexpage to display upcoming matches.
    [AllowAnonymous]
    [HttpGet("latest/{amount:int}")]
    public async Task<IActionResult> GetLatest(int amount)
    {
      int maxAmount = 20;
      if (amount > maxAmount) {
        amount = maxAmount;
      }

      try
      {
        List<ExtendedCompanyEventDto> companyEvents = await _companyEventRepository.GetLatestCompanyEvents(amount);
        if (companyEvents.Count <= 0)
        {
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "Just nu är det inga matcher på gång.",
            Data = null,
          });
        }
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = "",
          Data = companyEvents,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Ett problem uppstod när matcherna skulle hämtas.",
          Data = null,
        });
      }
    }

    [Authorize(Roles = "RefereeUser,CompanyUser,Admin")]
    [HttpGet("all/{page:int}")]
    public async Task<IActionResult> GetAllPaginate(int page)
    {
      try
      {
        List<ExtendedCompanyEventDto> companyEvents = await _companyEventRepository.GetAllEventsPage(page);
        if (companyEvents.Count <= 0)
        {
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "Inga matcher hittades från dagens datum och frammåt.",
            Data = null,
          });
        }
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = "",
          Data = companyEvents,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Ett problem uppstod när matcherna skulle hämtas.",
          Data = null,
        });
      }
    }

    [Authorize(Roles = "RefereeUser,CompanyUser,Admin")]
    [HttpPost("filtered")]
    public async Task<IActionResult> GetAllPaginateFiltered(CompanyEventsFiltered model)
    {
      try
      {
        List<ExtendedCompanyEventDto> companyEvents = await _companyEventRepository.GetFilteredEventsPage(model);
        if (companyEvents.Count <= 0)
        {
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "Inga fler matcher hittades från dagens datum och framåt med den filtreringen.",
            Data = null,
          });
        }
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = "",
          Data = companyEvents,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Ett problem uppstod när matcherna skulle hämtas.",
          Data = null,
        });
      }
    }

    [Authorize(Roles = "CompanyUser,Admin")]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create([FromBody] CreateCompanyEventBody request)
    {
      try
      {
        // In this case the claim will be companyId for the company user.
        var claimId = User.Identity.GetUserClaimId();
        var result = await _companyEventRepository.AddCompanyEvent(request, claimId);
        if (result)
        {
          _logger.LogInformation($"CompanyEvent created for company {claimId} with the name {request.Name}");
          return StatusCode(200, new ApiResponse
          {
            Success = true,
            Message = "CompanyEvent created",
            Data = null,
          });
        }
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Problem creating CompanyEvent",
          Data = null,
        });
      }
      catch (Exception)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Invalid request creating Companyevent",
          Data = null,
        });
      }
    }

    [Authorize(Roles = "CompanyUser,Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
      System.Console.WriteLine("GOT ID " + id);
      // TODO: Check if the companyId matches
      try
      {
        bool deleteResult = await _companyEventRepository.DeleteCompanyEventById(id);
        if (!deleteResult)
        {
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "The match could not be deleted",
            Data = null,
          });
        }
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = "The event was deleted",
          Data = null,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "There was a error when deleting the match",
          Data = null,
        });
      }
    }
  }
}
