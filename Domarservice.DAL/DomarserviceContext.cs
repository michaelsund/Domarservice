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

    public DbSet<Sport> Sports { get; set; }
    public DbSet<County> Counties { get; set; }
  }

  public class Referee
  {
    public int Id { get; set; }
    public string Surname { get; set; }
    public string Lastname { get; set; }
    public virtual List<Schedule> Schedules { get; set; }
    public virtual List<Sport> Sports { get; set; }
    public virtual List<County> Counties { get; set; }
  }

  public class Schedule
  {
    public int Id { get; set; }
    public DateTime AvailableAt { get; set; }
    public bool Booked { get; set; }
    public virtual Company ClaimedByCompany { get; set; }
    public int RefereeId { get; set; }
    public virtual Referee Referee { get; set; }
    public virtual List<BookingRequest> BookingRequests { get; set; }
  }

  public class Company
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Sport { get; set; }
    public virtual List<BookingRequest> BookingRequests { get; set; }
  }

  public enum SportType
  {
    Hockey,
    Fotboll,
    Innebandy
  }

  public enum RefereeType
  {
    Linjeman,
    Hudvuddomare
  }

  public class Sport
  {
    public int Id { get; set; }
    public RefereeType RefereeType { get; set; }
    public SportType SportType { get; set; }
    public int RefereeId { get; set; }
  }

  public class BookingRequest
  {
    public int Id { get; set; }
    public string Message { get; set; }
    public int CompanyId { get; set; }
    public virtual Company RequestingCompany { get; set; }
    public int ScheduleId { get; set; }
    public virtual Schedule Schedule { get; set; }
    public bool Accepted { get; set; }
    public DateTime RespondedAt { get; set; }
  }

  public class CountyType
  {
    public int Id { get; set; }
    public static string Norrbotten = "Norrbotten";
    public static string Vasterbotten = "Västerbotten";
    public static string Jamtland = "Jämtland";
    public static string Vasternorrland = "Västernorrland";
    public static string Dalarna = "Dalarna";
    public static string Gavleborg = "Gävleborg";
    public static string Varmalnd = "Värmland";
    public static string Orebro = "Örebro";
    public static string Vastmanland = "Västmanland";
    public static string Uppsala = "Uppsala";
    public static string Stockholm = "Stockholm";
    public static string Sodermanland = "Södermanland";
    public static string Vastragotaland = "Västra götaland";
    public static string Ostergotland = "Öster götaland";
    public static string Jonkoping = "Jönköping";
    public static string Kalmar = "Kalmar";
    public static string Kronoberg = "Kronoberg";
    public static string Halland = "Halland";
    public static string Skane = "Skåne";
    public static string Blekinge = "Blekinge";
    public static string Gotland = "Gotland";
  }

  // [Index(IsUnique = true)]
  public class County
  {
    public int Id { get; set; }
    public int RefereeId { get; set; }
    public string CountyName { get; set; }
  }
}