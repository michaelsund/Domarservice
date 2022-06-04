using System.Collections.Generic;
using Domarservice.DAL;

namespace Domarservice.Helpers
{
  public class RegisterCompanyModel
  {
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneOne { get; set; }
    public string PhoneTwo { get; set; }
    public string Email { get; set; }
    public string City { get; set; }
    public CountyType County { get; set; }
    public List<CompanySport> Sports { get; set; }
  }
}