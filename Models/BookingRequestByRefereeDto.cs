using System;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class BookingRequestByRefereeDto
  {
    public int Id { get; set; }
    public string Message { get; set; }
    public int RefereeId { get; set; }
    public RefereeType RefereeType { get; set; }
    public int CompanyEventId { get; set; }
    public bool Accepted { get; set; }
    public DateTime AppliedAt { get; set; }
  }
}