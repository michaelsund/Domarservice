using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Domarservice.DAL;
using Domarservice.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Domarservice.Controllers
{
  [ApiController]
  [Authorize]
  [Route("[controller]")]
  public class RefereeController : ControllerBase
  {
    private readonly IRefereeRepository _refereeRepository;
    private readonly ILogger _logger;

    public RefereeController(IRefereeRepository refereeRepository, ILogger<RefereeController> logger)
    {
      _refereeRepository = refereeRepository;
      _logger = logger;
    }

    [Authorize(Roles = "RefereeUser,CompanyUser,Admin")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
      try
      {
        var referee = await _refereeRepository.GetRefeereById(id);
        if (referee == null)
        {
          return StatusCode(500, new ApiResponse {
            Success = false,
            Message = "Could not find the referee.",
            Data = null
          });
        }
        return StatusCode(200, new ApiResponse {
            Success = true,
            Message = "Referee found.",
            Data = referee
          });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "There was an error finding the referee",
          Data = null,
        });
      }

    }

    [Authorize(Roles = "RefereeUser,Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        // TODO: Check that the refereeuser is the correct id in token
        var referee = await _refereeRepository.GetRefeereById(id);
        bool deleteResult = await _refereeRepository.DeleteRefereeById(id);
        if (!deleteResult)
        {
          return StatusCode(500, new ApiResponse
          {
            Success = false,
            Message = "The referee could not be deleted",
            Data = null,
          });
        }
        _logger.LogWarning($"The user {User.Identity.Name} deleted the referee {referee.Surname} {referee.Lastname} with id: {id}");
        return StatusCode(200, new ApiResponse
        {
          Success = true,
          Message = "The referee was deleted",
          Data = null,
        });
      }
      catch (Exception e)
      {
        return StatusCode(500, new ApiResponse
        {
          Success = false,
          Message = "There was a problem deleting the referee",
          Data = null,
        });
      }
    }
  }
}
