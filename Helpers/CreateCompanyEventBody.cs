using System;
using System.Collections.Generic;
using Domarservice.DAL;

namespace Domarservice.Helpers
{
  public class CreateCompanyEventBody
  {
    public string Name { get; set; }
    public string Location { get; set; }
    public int DurationHours { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime Date { get; set; }
    public SportType SportType { get; set; }
    public List<RefereeTypesCompanyEvent> RefereeTypesForEvent { get; set; }
  }
}