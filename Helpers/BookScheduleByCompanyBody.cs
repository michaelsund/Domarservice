using Domarservice.DAL;

namespace Domarservice.Helpers
{
  public class BookScheduleByCompanyBody
  {
    public int CompanyId { get; set; }
    public int ScheduleId { get; set; }
    public string Message { get; set; }
    public RefereeType RefereeType { get; set; }
    public SportType SportType { get; set; }
    public int CompanyEventId { get; set; }
  }
}