using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Domarservice.DAL;

namespace Domarservice.BLL
{
  public class DoTest
  {
    private readonly DomarserviceContext _context;
    public DoTest(DomarserviceContext context)
    {
      _context = context;
    }
    public void AddReferee(string surname, string lastname, int i)
    {
      _context.Referees.Add(new Referee()
      {
        Email = surname + i + "@" + lastname + ".com",
        Password = surname + i,
        Lastname = lastname,
        Surname = surname,
        Sports = new List<RefereeSport>() {
          new RefereeSport() { SportType = SportType.Fotboll, RefereeType = RefereeType.Hudvuddomare },
          new RefereeSport() { SportType = SportType.Ishockey, RefereeType = RefereeType.Linjeman },
        }
      });
      _context.SaveChanges();
    }
    public void AddScheduleDate(int id)
    {
      Referee referee = _context.Referees.Where(x => x.Id == id).FirstOrDefault();
      if (referee != null)
      {
        var s = new Schedule()
        {
          RefereeId = referee.Id,
          Referee = referee,
          AvailableAt = DateTime.UtcNow,
        };
        _context.Schedules.Add(s);
        _context.SaveChanges();
      }
    }

    public void AddCompanyAndScheduleFirstReferee(string name, int y)
    {
      _context.Companies.Add(new Company()
      {
        Name = name,
        Address = name + " gatan " + y,
        Email = name + y + "@" + name + ".com",
        Password = name + y,
        City = name,
        PhoneOne = "019123456",
        PhoneTwo = "019654321",
        Sports = new List<CompanySport>() {
          new CompanySport() { SportType = SportType.Fotboll },
          new CompanySport() { SportType = SportType.Ishockey },
        }
      });
      _context.SaveChanges();

      var c = _context.Companies.Where(x => x.Id == y).FirstOrDefault();
      var s = _context.Schedules.Where(x => x.Id == y).FirstOrDefault();

      if (c != null && s != null)
      {
        _context.BookingRequestsByCompany.Add(new BookingRequestByCompany()
        {
          CompanyId = c.Id,
          Message = "Kan du komma på vår match?",
          RefereeType = RefereeType.Hudvuddomare,
          SportType = SportType.Fotboll,
          RequestingCompany = c,
          Schedule = s,
          ScheduleId = s.Id
        });
      }
      else
      {
        System.Console.WriteLine("Company or Schedule not found by ID!");
      }
      _context.SaveChanges();
    }

    public void AddEventForCompany(string name, int y)
    {
      _context.CompanyEvents.Add(new CompanyEvent()
      {
        Name = name,
        Date = DateTime.UtcNow,
        Location = "Ölands jumpasal",
        CompanyId = y,
        RefereeTypes = new List<RefereeType>() {
          RefereeType.Hudvuddomare,
          RefereeType.Linjeman,
          RefereeType.Linjeman,
        }
      });
      _context.SaveChanges();
    }

    public void AddRefereeForEvent(string message, int y)
    {
      _context.BookingRequestsByReferee.Add(new BookingRequestByReferee() {
        CompanyEventId = y,
        RefereeId = y,
        Message = message,
        AppliedAt = DateTime.UtcNow,
      });
    }

    public bool RespondYes(int y)
    {
      var s = _context.Schedules.Where(x => x.Id == y).FirstOrDefault();
      var r = _context.BookingRequestsByCompany.Where(x => x.Id == y).FirstOrDefault();

      if (s != null && r != null)
      {
        r.Accepted = true;
        r.RespondedAt = DateTime.UtcNow;
        _context.SaveChanges();
      }

      return true;
    }

    public bool AddCounty(int y)
    {
      Referee referee = _context.Referees.Where(x => x.Id == y).FirstOrDefault();
      var countys = new List<County>() {
        new County() { RefereeId = y, CountyType = CountyType.Orebro },
        new County() { RefereeId = y, CountyType = CountyType.Varmland },
      };
      if (referee != null)
      {
        referee.Countys = countys;
        _context.SaveChanges();
      }

      return true;
    }
  }
}