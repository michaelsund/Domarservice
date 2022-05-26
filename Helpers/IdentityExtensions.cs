using System;
using System.Security.Claims;
using System.Security.Principal;
using Serilog;

namespace Domarservice.Helpers
{
  public static class IdentityExtensions
  {

    public static int GetUserClaimId(this IIdentity identity)
    {
      ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
      try
      {
        Claim claim = claimsIdentity?.FindFirst(CustomClaimTypes.Id);
        if (claim == null)
        {
          return 0;
        }

        return int.Parse(claim.Value);
      }
      catch (Exception e)
      {
        Log.Error(e, "Problem reading and parsing the tokens \"Id\" Claim, it was probably null in jwt token.");
        return 0;
      }
    }

    public static string GetUserClaimName(this IIdentity identity)
    {
      ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
      Claim claim = claimsIdentity?.FindFirst(ClaimTypes.Name);

      return claim?.Value ?? string.Empty;
    }

    public static bool CheckAdminRole(this IIdentity identity)
    {
      ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
      Claim claim = claimsIdentity?.FindFirst(ClaimTypes.Role);
      var isAdmin = claim?.Value == "Admin" ? true : false;
      return isAdmin;
    }
  }
}
