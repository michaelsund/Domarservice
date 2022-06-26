using Domarservice.Helpers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Domarservice.BLL
{
  public interface IAdministrationService
  {
    Task<bool> AssignUserToRole(RoleBody model);
    Task<bool> RemoveUserRole(RoleBody model);
    Task<List<string>> GetUserRoles(string email);
    Task<bool> DeleteUser(string email);
    Task<bool> DeleteUserAndReferee(string email);
  }
}
