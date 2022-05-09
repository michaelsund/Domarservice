using System;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class ScheduleDto
  {
    public string AvailableAt { get; set; }
    public bool Booked { get; set; }
    public Company ClaimedByCompany { get; set; }
  }
}