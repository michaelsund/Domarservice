using System;
using System.Collections.Generic;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class ScheduleDto
  {
    public int Id { get; set; }
    public SimpleRefereeDto Referee { get; set; }
    public string AvailableAt { get; set; }
    public List<BookingRequestByCompanyDto> BookingRequestByCompanys { get; set; }
  }
}