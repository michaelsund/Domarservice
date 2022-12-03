using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Domarservice.DAL;
using Domarservice.Helpers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Domarservice.BLL
{
  public class AdministrationService : IAdministrationService
  {
    private readonly ILogger _logger;
    private readonly ISendMailHelper _sendMailHelper;
    private readonly IRefereeRepository _refereeRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    public AdministrationService(
      ILogger<AdministrationService> logger,
      ISendMailHelper sendMailHelper,
      IRefereeRepository refereeRepository,
      ICompanyRepository companyRepository,
      UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager,
      IConfiguration configuration
    )
    {
      _logger = logger;
      _sendMailHelper = sendMailHelper;
      _refereeRepository = refereeRepository;
      _companyRepository = companyRepository;
      _userManager = userManager;
      _roleManager = roleManager;
      _configuration = configuration;
    }

    public async Task<bool> DeleteUser(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user != null)
      {
        var deleteResult = await _userManager.DeleteAsync(user);
        if (deleteResult.Succeeded)
        {
          return true;
        }
      }
      return false;
    }

    // This is the "remove me" for a user that is a referee.
    public async Task<bool> DeleteUserAndReferee(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user != null)
      {
        var deleteResult = await _userManager.DeleteAsync(user);
        if (deleteResult.Succeeded)
        {
          // Then remove the referee tied to the user
          if (user.RefereeId != null)
          {
            var referee = await _refereeRepository.DeleteRefereeById(user.RefereeId);
          }
          return true;
        }
      }
      return false;
    }

    // Company can have multiple users? Should we really delete a company since it's never a person with GDPR information.

    // public async Task<bool> DeleteUserAndCompany(string email)
    // {
    //   var user = await _userManager.FindByEmailAsync(email);
    //   if (user != null)
    //   {
    //     var deleteResult = await _userManager.DeleteAsync(user);
    //     if (deleteResult.Succeeded)
    //     {
    //       // Then remove the company tied to the user
    //       if (user.CompanyId != null)
    //       {
    //         var referee = await _refereeRepository.DeleteRefereeById(user.RefereeId);
    //       }
    //       return true;
    //     }
    //   }
    //   return false;
    // }

    public async Task<bool> AssignUserToRole(RoleBody model)
    {
      try
      {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
          _logger.LogWarning($"{model.Email} attempted to be added to {model.Role} with Company/Referee ID {model.BindToModelId}, but the user could not be found.");
          return false;
        }

        // Check that a existing Id doesnt exist yet for Company or Referee on the User object
        // if (user.CompanyId == null && user.RefereeId == null)
        // {

        // Then bind to correct id
        if (model.Role == UserRoles.RefereeUser)
        {
          user.RefereeId = model.BindToModelId;
          user.CompanyId = null;
        }
        else if (model.Role == UserRoles.CompanyUser)
        {
          user.CompanyId = model.BindToModelId;
          user.RefereeId = null;
        }
        else if (model.Role == UserRoles.Admin)
        {
          // Admin doesnt use these fields
          user.CompanyId = null;
          user.RefereeId = null;
        }

        await _userManager.UpdateAsync(user);
        var result = await _userManager.AddToRoleAsync(user, model.Role);
        if (result.Succeeded)
        {
          _logger.LogInformation($"{model.Email} was added to the role {model.Role} with Company/Referee ID {model.BindToModelId}");
          return true;
        }
        // }
        // _logger.LogWarning($"{model.Email} Allready has a CompanyId or RefereeId, cannot set new Role with Company/Referee binding with role {model.Role} to ID {model.BindToModelId}.");
        return false;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> RemoveUserRole(RoleBody model)
    {
      try
      {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
          return false;
        }

        // Remove company and referee bindings when unsetting roles for users.
        if (model.Role == UserRoles.CompanyUser)
        {
          user.CompanyId = null;
        }
        else if (model.Role == UserRoles.RefereeUser)
        {
          user.RefereeId = null;
        }

        await _userManager.UpdateAsync(user);
        var result = await _userManager.RemoveFromRoleAsync(user, model.Role);
        if (result.Succeeded)
        {
          return true;
        }
        return false;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<List<string>> GetUserRoles(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user != null)
      {
        var roles = await _userManager.GetRolesAsync(user);
        List<string> roleList = new List<string>();
        foreach (var role in roles)
        {
          roleList.Add(role);
        }
        return roleList;
      }
      return new List<string>();
    }
  }
}