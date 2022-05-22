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
    public DbSet<BookingRequestByCompany> BookingRequestsByCompany { get; set; }
    public DbSet<BookingRequestByReferee> BookingRequestsByReferee { get; set; }
    public DbSet<CompanyEvent> CompanyEvents { get; set; }
    public DbSet<CompanySport> CompanySports { get; set; }
    public DbSet<RefereeSport> RefereeSports { get; set; }

    public DbSet<County> Countys { get; set; }
  }

  public class Referee
  {
    public int Id { get; set; }
    public string Surname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<Schedule> Schedules { get; set; }
    public List<RefereeSport> Sports { get; set; }
    public List<County> Countys { get; set; }
  }

  public class Schedule
  {
    public int Id { get; set; }
    public DateTime AvailableAt { get; set; }
    public int RefereeId { get; set; }
    public Referee Referee { get; set; }
    public List<BookingRequestByCompany> BookingRequestByCompanys { get; set; }
  }

  public class Company
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneOne { get; set; }
    public string PhoneTwo { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string City { get; set; }
    public CountyType County { get; set; }
    public List<CompanySport> Sports { get; set; }
    public List<CompanyEvent> CompanyEvents { get; set; }
  }

  public class CompanyEvent
  {
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public DateTime Date { get; set; }
    public SportType SportType { get; set; }
    public List<RefereeType> RefereeTypes { get; set; }
    public List<BookingRequestByReferee> BookingRequestByReferees { get; set; }
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
    public int CompanyId { get; set; }

  }

  public class BookingRequestByCompany
  {
    public int Id { get; set; }
    public string Message { get; set; }
    public SportType SportType { get; set; }
    public RefereeType RefereeType { get; set; }
    public int CompanyId { get; set; }
    public Company RequestingCompany { get; set; }
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; }
    // No relation, is set when a company books a referee and have a match via CompanyEvent.
    public int RequestingCompanyEventId { get; set; }
    public bool Accepted { get; set; }
    public DateTime RespondedAt { get; set; }
  }

   public class BookingRequestByReferee
  {
    public int Id { get; set; }
    public string Message { get; set; }
    public int RefereeId { get; set; }
    public RefereeType RefereeType { get; set; }
    public int CompanyEventId { get; set; }
    public CompanyEvent CompanyEvent { get; set; }
    public bool Accepted { get; set; }
    public DateTime AppliedAt { get; set; }
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