using System;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class SportDto
  {
    public RefereeType RefereeType { get; set; }
    public SportType SportType { get; set; }
  }
}