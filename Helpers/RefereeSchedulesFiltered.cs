using System;
using System.Collections.Generic;
using Domarservice.DAL;
using Domarservice.Models;

namespace Domarservice.Helpers
{
  public class RefereeSchedulesFiltered
  {
    public int Page { get; set; }
    public SimpleRefereeDto Referee { get; set; }
    public CountyType[] CountysFilter { get; set; }
    public SportType[] SportsFilter { get; set; }
    public RefereeType[] RefereeTypeFilter { get; set; }
    // public string CompanySearchString { get; set; }
    public DateTime AvailableFromDate { get; set; }
  }
}