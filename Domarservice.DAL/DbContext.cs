using System;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Domarservice.DAL
{
  public class DomarserviceContext : DbContext
  {
    public DomarserviceContext(DbContextOptions<DomarserviceContext> options) : base(options) { }
    public DbSet<Referee> Referees { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<BookingRequest> BookingRequests { get; set; }

  }

  public class Referee
  {
    public int Id { get; set; }
    public string Surname { get; set; }
    public string Lastname { get; set; }
    public List<Schedule> Schedules { get; set; }
  }

  public class Schedule
  {
    public int Id { get; set; }
    public DateTime AvailableAt { get; set; }
    public bool Booked { get; set; }
    public Company ClaimedByCompany { get; set; }
    public int RefereeId { get; set; }
    public Referee Referee { get; set; }
    public List<BookingRequest> BookingRequests { get; set; }
  }

  public class Company
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Sport { get; set; }
    public List<BookingRequest> BookingRequests { get; set; }
  }

  public class BookingRequest
  {
    public int Id { get; set; }
    public string Message { get; set; }
    public int CompanyId { get; set; }
    public Company RequestingCompany { get; set; }
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; }
    public bool Accepted { get; set; }
    public DateTime RespondedAt { get; set; }
  }
}