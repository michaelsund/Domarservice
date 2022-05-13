using System;
using System.Collections.Generic;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class ScheduleDto
  {
    public string AvailableAt { get; set; }
    public bool Booked { get; set; }
    public CompanyDto ClaimedByCompany { get; set; }
    public List<BookingRequestDto> BookingRequests { get; set; }
  }
}