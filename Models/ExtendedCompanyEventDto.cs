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
    public SportType SportType { get; set; }
    public SimpleCompanyDto Company { get; set; }
    public List<RefereeTypesCompanyEventDto> RefereeTypesForEvent { get; set; }
  }
}