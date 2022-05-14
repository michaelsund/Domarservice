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
  public class CompanyEventController : ControllerBase
  {
    private readonly ICompanyEventRepository _companyEventRepository;
    public CompanyEventController(ICompanyEventRepository companyEventRepository)
    {
      _companyEventRepository = companyEventRepository;
    }

   [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
      var companyEvent = await _companyEventRepository.GetCompanyEventById(id);
      if (companyEvent == null)
      {
        return NotFound("Matchen hittades inte.");
      }
      return Ok(companyEvent);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
      bool deleteResult = await _companyEventRepository.DeleteCompanyEventById(id);
      if (!deleteResult)
      {
        return StatusCode(StatusCodes.Status400BadRequest, "Matchen kunde inte tas bort.");
      }
      return Ok();
    }
  }
}
