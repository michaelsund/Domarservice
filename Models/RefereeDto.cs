using System;
using System.Collections.Generic;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class RefereeDto
  {
    public string Surname { get; set; }
    public string Lastname { get; set; }
    public List<Sport> Sports { get; set; }
    // public List<Schedule> Schedules { get; set; }
    // public List<County> Counties { get; set; }
  }
}