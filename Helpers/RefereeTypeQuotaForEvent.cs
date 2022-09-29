using System.Linq;
using Domarservice.DAL;

namespace Domarservice.Helpers
{
  public class RefereeTypeQuotaForEvent
  {
    public ResultWithMessage Check(CompanyEvent companyEvent, BookCompanyEventByRefereeBody incomingRequest)
    {
      if (companyEvent.BookingRequestByReferees.Count > 0)
      {
        var acceptedRoles = companyEvent.BookingRequestByReferees.Where(x => x.Accepted).ToList();
        int numRefereeMain = 0;
        int numRefereeLineMan = 0;

        foreach (var refereeType in companyEvent.RefereeTypesForEvent)
        {
          if (refereeType.RefereeType == RefereeType.Hudvuddomare)
          {
            numRefereeMain += 1;
          }

          if (refereeType.RefereeType == RefereeType.Linjeman)
          {
            numRefereeLineMan += 1;
          }
        }

        foreach (var acceptedRole in acceptedRoles)
        {
          if (acceptedRole.RefereeType == RefereeType.Hudvuddomare)
          {
            numRefereeMain -= 1;
          }

          if (acceptedRole.RefereeType == RefereeType.Linjeman)
          {
            numRefereeLineMan -= 1;
          }
        }

        if (incomingRequest.RefereeType == RefereeType.Hudvuddomare && numRefereeMain > 0)
        {
          // Better response here why there is no room left.
          return new ResultWithMessage
          {
            Result = true,
            Message = "Intresseanmälan som huvuddomare.",
            Data = null
          };
        }

        if (incomingRequest.RefereeType == RefereeType.Linjeman && numRefereeLineMan > 0)
        {
          // Better response here why there is no room left.
          return new ResultWithMessage
          {
            Result = true,
            Message = "Intresseanmälan skickad.",
            Data = null
          };
        }

        return new ResultWithMessage
        {
          Result = false,
          Message = $"Platserna för rollen {incomingRequest.RefereeType.ToString().ToLower()} inte tillgänglig.",
          Data = null
        };
      }

      return new ResultWithMessage
      {
        Result = true,
        Message = "Intresseanmälan skickad.",
        Data = null
      };
    }
  }
}