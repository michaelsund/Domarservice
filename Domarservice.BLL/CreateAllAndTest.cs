using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.DAL;

namespace Domarservice.BLL
{
  public class CreateAllAndTest
  {
    private readonly DomarserviceContext _context;
    public CreateAllAndTest(DomarserviceContext context)
    {
      _context = context;
    }
    public void RunCreate()
    {
      try
      {
        _context.Companies.Add(new Company()
        {
          Name = "Smygehuk BK",
          Sport = "Fotboll",
        });
        _context.Referees.Add(new Referee()
        {
          Lastname = "Knutsson",
          Surname = "Örjan",
          ScheduleList = new List<Schedule>(),
        });
        _context.SaveChangesAsync();
      }
      catch (System.Exception)
      {
        throw;
      }

      // try
      // {
      //   DateTime localDate = DateTime.Now;
      //   var referee = _context.Referees.Single(x => x.Id == 2);
      //   if (referee != null)
      //   {
      //     referee.ScheduleList.Add(new Schedule()
      //     {
      //       AvailableAt = localDate,
      //       Booked = false,
      //     });
      //   }
      // }
      // catch (System.Exception)
      // {

      //   throw;
      // }
    }
  }
}