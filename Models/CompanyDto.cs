using System;
using System.Collections.Generic;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class CompanyDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public CountyType County { get; set; }
    public List<CompanySportDto> Sports { get; set; }
    public List<CompanyEventDto> CompanyEvents { get; set; }
  }
}