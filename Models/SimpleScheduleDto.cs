using System;
using System.Collections.Generic;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class SimpleScheduleDto
  {
    public int Id { get; set; }
    public string AvailableAt { get; set; }
    public List<BookingRequestByCompanyDto> BookingRequestByCompanys { get; set; }
  }
}