using System;
using System.Collections.Generic;

namespace Domarservice.Models
{
  public class RefereeDto
  {
    public string Surname { get; set; }
    public string Lastname { get; set; }
    public List<RefereeSportDto> Sports { get; set; }
    public List<ScheduleDto> Schedules { get; set; }
    public List<CountyDto> Countys { get; set; }
  }
}