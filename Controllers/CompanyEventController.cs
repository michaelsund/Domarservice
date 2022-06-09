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
          return NotFound("Can not find event.");
        }
        return Ok(companyEvent);
      }
      catch (Exception e)
      {
        return StatusCode(500);
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
          return Ok("CompanyEvent created.");
        }
        return StatusCode(500, "Problem creating CompanyEvent");
      }
      catch (Exception)
      {
        return StatusCode(500, "Invalid request creating CompanyEvent");
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
          return StatusCode(StatusCodes.Status400BadRequest, "The match could not be deleted.");
        }
        return Ok();
      }
      catch (Exception e)
      {
        return StatusCode(500);
      }
    }
  }
}
