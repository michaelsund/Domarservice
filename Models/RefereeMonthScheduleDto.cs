using System;
using System.Collections.Generic;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class RefereeMonthScheduleDto
  {
    public int Id { get; set; }
    public int Day { get; set; }
    public string DayName { get; set; }
    public int Week { get; set; }
    public List<Available> AvailableTimes { get; set; }
    public List<BookingRequestByCompanyDto> BookingRequestByCompanys { get; set; }
  }
}