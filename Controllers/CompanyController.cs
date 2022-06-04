using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Domarservice.DAL;
using Domarservice.Helpers;

namespace Domarservice.Controllers
{
  [Authorize(Roles = "CompanyUser,Admin")]
  [ApiController]
  [Route("[controller]")]
  public class CompanyController : ControllerBase
  {
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger _logger;

    public CompanyController(ICompanyRepository companyRepository, ILogger<CompanyController> logger)
    {
      _companyRepository = companyRepository;
      _logger = logger;
    }

    [HttpPost]
    [Route("register-company")]
    public async Task<IActionResult> RegisterCompany(RegisterCompanyModel model)
    {
      try
      {
        var result = await _companyRepository.AddNewCompany(model);
        if (result)
        {
          _logger.LogInformation($"The following company was created: {model}");
          return Ok($"Föreningen {model.Name} har skapats.");
        }
        return StatusCode(500, "The company could not be created, please try again.");
      }
      catch (Exception e)
      {
        _logger.LogWarning($"Something went wrong while trying to create the company {model.Name}. {e}");
        return StatusCode(500, $"Something went wrong while trying to create the company.");
      }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
      try
      {
        var company = await _companyRepository.GetSimpleCompanyById(id);
        if (company == null)
        {
          return NotFound("Föreningen hittades inte.");
        }
        return Ok(company);
      }
      catch (Exception e)
      {
        return StatusCode(500);
      }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        bool deleteResult = await _companyRepository.DeleteCompanyById(id);
        if (!deleteResult)
        {
          return StatusCode(StatusCodes.Status400BadRequest, "Company could not be deleted.");
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
