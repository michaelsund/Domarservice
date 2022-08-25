using System;
using System.Collections.Generic;
using Domarservice.DAL;

namespace Domarservice.Helpers
{
  public class CompanyEventsFiltered
  {
    public int Page { get; set; }
    public CountyType[] CountysFilter { get; set; }
    public SportType[] SportsFilter { get; set; }
    public RefereeType[] RefereesFilter { get; set; }
    public string CompanySearchString { get; set; }
    public DateTime FromDate { get; set; }
  }
}