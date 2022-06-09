using Domarservice.DAL;

namespace Domarservice.Helpers
{
  public class BookCompanyEventByRefereeBody
  {
    public int CompanyEventId { get; set; }
    public string Message { get; set; }
    public RefereeType RefereeType { get; set; }
  }
}