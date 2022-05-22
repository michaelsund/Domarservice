using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Domarservice.DAL;
using Domarservice.BLL;
using Domarservice.Models;

namespace Domarservice.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class CompanyController : ControllerBase
  {
    private readonly ICompanyRepository _companyRepository;
    public CompanyController(ICompanyRepository companyRepository)
    {
      _companyRepository = companyRepository;
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
          return StatusCode(StatusCodes.Status400BadRequest, "Föreningen kunde inte tas bort.");
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
