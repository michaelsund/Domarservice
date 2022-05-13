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

    public DbSet<CompanySport> CompanySports { get; set; }
    public DbSet<RefereeSport> RefereeSports { get; set; }

    public DbSet<County> Countys { get; set; }
  }

  public class Referee
  {
    public int Id { get; set; }
    public string Surname { get; set; }
    public string Lastname { get; set; }
    public List<Schedule> Schedules { get; set; }
    public List<RefereeSport> Sports { get; set; }
    public List<County> Countys { get; set; }
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
    public List<CompanySport> Sports { get; set; }
    public List<BookingRequest> BookingRequests { get; set; }
  }

  public enum SportType
  {
    Ishockey,
    Fotboll,
    Innebandy
  }

  public enum RefereeType
  {
    Linjeman,
    Hudvuddomare
  }

  public class RefereeSport
  {
    public int Id { get; set; }
    public RefereeType RefereeType { get; set; }
    public SportType SportType { get; set; }
    public int RefereeId { get; set; }
  }

  public class CompanySport
  {
    public int Id { get; set; }
    public SportType SportType { get; set; }
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

  public enum CountyType
  {
    Norrbotten,
    Vasterbotten,
    Jamtland,
    Vasternorrland,
    Dalarna,
    Gavleborg,
    Varmland,
    Orebro,
    Vastmanland,
    Uppsala,
    Stockholm,
    Sodermanland,
    Vastragotaland,
    Ostergotland,
    Jonkoping,
    Kalmar,
    Kronoberg,
    Halland,
    Skane,
    Blekinge,
    Gotland,
  }

  public class County
  {
    public int Id { get; set; }
    public CountyType CountyType { get; set; }
    public int RefereeId { get; set; }

  }
}