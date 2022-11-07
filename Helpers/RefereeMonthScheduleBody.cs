using Domarservice.DAL;

namespace Domarservice.Helpers
{
  public class RefereeMonthScheduleBody
  {
    public int RefereeId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
  }
}