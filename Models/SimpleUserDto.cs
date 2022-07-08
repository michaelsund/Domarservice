using System;
using System.Collections.Generic;

namespace Domarservice.Models
{
  public class SimpleUserDto
  {
    // Id is a guid..
    // public int Id { get; set; }
    public string Surname { get; set; }
    public string Lastname { get; set; }
    public string Information { get; set; }
    public string Email { get; set; }
    public string PhoneOne { get; set; }
    public string PhoneTwo { get; set; }
  }
}