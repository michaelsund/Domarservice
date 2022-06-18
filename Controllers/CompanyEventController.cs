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

namespace Domarservice.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class CompanyEventController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly ICompanyEventRepository _companyEventRepository;
    public CompanyEventController(ILogger<ScheduleController> logger, ICompanyEventRepository companyEventRepository)
    {
      _logger = logger;
      _companyEventRepository = companyEventRepository;
    }

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
            Message = "Cannot find event",
            Data = null,
          });
        }
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = "Here is your event",
          Data = companyEvent,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Error fetching company event",
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
          Message = "The match was created",
          Data = null,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "There was a error when creating the match",
          Data = null,
        });
      }
    }
  }
}
