using System;
using Domarservice.DAL;

namespace Domarservice.Models
{
  public class Available
  {
    public int Id { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
  }
}