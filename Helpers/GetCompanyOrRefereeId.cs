using System;
using Domarservice.DAL;

namespace Domarservice.Helpers
{
  public class CompanyOrRefereeId
  {
    public int? Get(ApplicationUser user)
    {
      if (!String.IsNullOrEmpty(user.CompanyId.ToString()))
      {
        return user.CompanyId;
      }
      else if (!String.IsNullOrEmpty(user.RefereeId.ToString()))
      {
        return user.RefereeId;
      }
      return 0;
    }
  }
}
