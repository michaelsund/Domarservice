using System;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class BookingRequestByCompanyDto
  {
    public int Id { get; set; }
    public string Message { get; set; }
    public SimpleCompanyDto RequestingCompany { get; set; }
    public SportType SportType { get; set; }
    public RefereeType RefereeType { get; set; }
    public bool Accepted { get; set; }
    public DateTime RespondedAt { get; set; }
  }
}