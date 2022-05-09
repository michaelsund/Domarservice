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

      var sports = new List<Sport>();
      sports.Add(new Sport()
      {
        RefereeType = RefereeType.Hudvuddomare,
        SportType = SportType.Fotboll,
      });
      sports.Add(new Sport()
      {
        RefereeType = RefereeType.Linjeman,
        SportType = SportType.Fotboll,
      });

      _context.Referees.Add(new Referee()
      {
        Lastname = "Karlsson",
        Surname = "Kalle",
        Sports = sports
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
      var counties = new List<County>() {
        new County() { RefereeId = y, CountyName = CountyType.Blekinge },
        new County() { RefereeId = y, CountyName = CountyType.Halland },
      };
      if (referee != null) {
        referee.Counties = counties;
        _context.SaveChanges();
      }

      return true;
    }

    public List<Schedule> GetSchedules(int id)
    {
      var schedulesForReferee = _context.Schedules
        .AsNoTracking()
        .Include(x => x.ClaimedByCompany)
        .Include(x => x.Referee)
        .Include(x => x.BookingRequests)
        .Where(x => x.RefereeId == id).ToList();
      // var schedulesForReferee = _context.Schedules
      //   .Select(schedule => new {
      //     schedule = schedule,
      //     claimedByCompany = schedule.ClaimedByCompany
      //   }).ToList();

      return schedulesForReferee;
    }

    public Referee GetReferee(int id)
    {
      var referee = _context.Referees
        .AsNoTracking()
        .Include(x => x.Schedules)
        .Include(x => x.Sports)
        .Include(x => x.Counties)
        .Where(x => x.Id == id).FirstOrDefault();

      return referee;
    }
  }
}