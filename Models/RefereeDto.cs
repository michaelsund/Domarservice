using System;
using System.Collections.Generic;

namespace Domarservice.Models
{
  public class RefereeDto
  {
    public int Id { get; set; }
    // Surname and lastname are picked in some cases from the connected user.
    public string Surname { get; set; }
    public string Lastname { get; set; }
    public List<RefereeSportDto> Sports { get; set; }
    public List<RefereeScheduleDto> Schedules { get; set; }
    public List<CountyDto> Countys { get; set; }
  }
}