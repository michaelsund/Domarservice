using System;

namespace Domarservice.Models
{
  public class BookingRequestDto
  {
    public int Id { get; set; }
    public string Message { get; set; }
    public CompanyDto RequestingCompany { get; set; }
    public bool Accepted { get; set; }
    public DateTime RespondedAt { get; set; }
  }
}