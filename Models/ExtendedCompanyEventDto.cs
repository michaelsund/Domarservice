using System;
using System.Collections.Generic;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class ExtendedCompanyEventDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public DateTime Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public SportType SportType { get; set; }
    public SimpleCompanyDto Company { get; set; }
    public List<RefereeTypesCompanyEventDto> RefereeTypesForEvent { get; set; }
    public List<BookingRequestByRefereeDto> BookingRequestByReferees { get; set; }
  }
}