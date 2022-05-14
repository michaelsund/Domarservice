using System;
using System.Collections.Generic;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class SimpleCompanyDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public List<CompanySportDto> Sports { get; set; }
  }
}