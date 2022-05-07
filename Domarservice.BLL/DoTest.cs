using System;
using System.Collections.Generic;
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
    public bool RunTest1()
    {
      try
      {
        _context.Referees.Add(new Referee()
        {
          Lastname = "Karlsson",
          Surname = "Kalle",
        });
        _context.SaveChanges();
      }
      catch (Exception e)
      {
        System.Console.WriteLine("Uh oh");
        throw;
      }

      return true;
    }
    public bool RunTest2()
    {
      Referee referee = _context.Referees.Where(x => x.Surname == "Kalle").FirstOrDefault();
      if (referee != null)
      {
        try
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
        catch (Exception e)
        {
          System.Console.WriteLine("Uh oh");
          throw;
        }
      }

      return true;
    }

    public bool RunTest3()
    {
      // Kalles schedules
      var s = _context.Schedules.Where(x => x.Referee.Surname == "Kalle").ToList();

      return true;
    }

    public bool RunTest4()
    {
      _context.Companies.Add(new Company()
      {
        Name = "Smygehuks IK",
        Sport = "Hockey",
      });
      _context.SaveChanges();

      var c = _context.Companies.Where(x => x.Name == "Smygehuks IK").FirstOrDefault();
      var s = _context.Schedules.Where(x => x.Id == 1).FirstOrDefault();

      if (c != null && s != null)
      {
        _context.BookingRequests.Add(new BookingRequest()
        {
          CompanyId = c.Id,
          Message = "Kan du komma på vår match Kalle?",
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

      return true;
    }

    public bool RunTest5()
    {
      var s = _context.Schedules.Where(x => x.Id == 1).FirstOrDefault();
      var r = _context.BookingRequests.Where(x => x.Id == 1).FirstOrDefault();

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

    public BookingRequest RunTest6()
    {
      var r = new BookingRequest();
      try
      {
        r = _context.BookingRequests.Where(x => x.Id == 1).FirstOrDefault();      
      }
      catch (System.Exception)
      {
        throw;
      }

      return r;
    }
  }

}