using System;
using System.Collections.Generic;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class RefereeScheduleDto
  {
    public int Id { get; set; }
    public int RefereeId { get; set; }
    public SimpleRefereeDto Referee { get; set; }
    public string AvailableAt { get; set; }
    public List<BookingRequestByCompanyDto> BookingRequestByCompanys { get; set; }
  }
}