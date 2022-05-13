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
    public void AddReferee(string surname, string lastname)
    {
      _context.Referees.Add(new Referee()
      {
        Lastname = "Karlsson",
        Surname = "Kalle",
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
          Booked = false,
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
        _context.BookingRequests.Add(new BookingRequest()
        {
          CompanyId = c.Id,
          Message = "Kan du komma på vår match?",
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

    public bool RespondYes(int y)
    {
      var s = _context.Schedules.Where(x => x.Id == y).FirstOrDefault();
      var r = _context.BookingRequests.Where(x => x.Id == y).FirstOrDefault();

      if (s != null)
      {
        s.Booked = true;
        r.Accepted = true;
        r.RespondedAt = DateTime.UtcNow;
        s.ClaimedByCompany = r.RequestingCompany;
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
      if (referee != null) {
        referee.Countys = countys;
        _context.SaveChanges();
      }

      return true;
    }
  }
}