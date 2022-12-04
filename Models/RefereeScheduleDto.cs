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
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public List<BookingRequestByCompanyDto> BookingRequestByCompanys { get; set; }
  }
}