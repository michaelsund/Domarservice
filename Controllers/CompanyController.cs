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
using Domarservice.Models;

namespace Domarservice.Controllers
{
  [Authorize]
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

    [Authorize(Roles = "CompanyUser")]
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
          return StatusCode(200, new ApiResponse
          {
            Success = true,
            Message = $"Föreningen {model.Name} har skapats.",
            Data = null,
          });
        }
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "The company could not be created, please try again.",
          Data = null,
        });
      }
      catch (Exception e)
      {
        _logger.LogWarning($"Something went wrong while trying to create the company {model.Name}. {e}");
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Something went wrong while trying to create the company",
          Data = null,
        });
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
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "Could not find the company",
            Data = null,
          });
        }
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = "The company was found",
          Data = company,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "There was a error finding the company",
          Data = null,
        });
      }
    }

    [HttpGet("{id:int}/users")]
    public async Task<IActionResult> GetCompanyUsers(int id)
    {
      try
      {
        List<SimpleUserDto> users = await _companyRepository.GetCompanyUsersByCompanyId(id);
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = "Användare som tillhör föreningen.",
          Data = users,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Kunde inte hämta användare till föreningen.",
          Data = null,
        });
      }
    }

    // This endpoint should never be called except in rare cases by admin.
    // Even if a user connected to this company object is deleted, the company should still exist.
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        var claimId = User.Identity.GetUserClaimId();
        // Check that the users claimId (companyId) is the companyId to be deleted.
        var company = await _companyRepository.GetCompanyById(id);
        if (claimId == id)
        {
          bool deleteResult = await _companyRepository.DeleteCompanyById(id);
          if (!deleteResult)
          {
            return StatusCode(500, new ApiResponse
            {
              Success = false,
              Message = "The company could not be deleted",
              Data = null,
            });
          }
          _logger.LogWarning($"The user {User.Identity.Name} deleted the company {company.Name} with id: {id}");
          return StatusCode(200, new ApiResponse
          {
            Success = true,
            Message = "The company was deleted",
            Data = null,
          });
        }
        else
        {
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "You do not have permission to delete this company",
            Data = null,
          });
        }
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "Error deleting the company",
          Data = null,
        });
      }
    }
  }
}
