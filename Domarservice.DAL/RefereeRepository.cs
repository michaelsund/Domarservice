using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Domarservice.Models;
using AutoMapper;

namespace Domarservice.DAL
{
  public class RefereeRepository : IRefereeRepository
  {
    private readonly DomarserviceContext _context = null;
    private readonly IMapper _mapper;

    public RefereeRepository(DomarserviceContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    // public List<Referee> GetAllReferees()
    // {
    //   var referees = _context.Referees.ToList();
    //   var model = _mapper.Map<RefereeDto>(referees);
    //   return model;
    // }

    public RefereeDto GetRefeereById(int id)
    {
      Referee referee = _context.Referees
        .Include(x => x.Sports)
        .Include(x => x.Countys)
        .Include(x => x.Schedules)
          .ThenInclude(y => y.ClaimedByCompany)
          .ThenInclude(y => y.BookingRequests)
        .FirstOrDefault(x => x.Id == id);
      var model = _mapper.Map<RefereeDto>(referee);

      return model;
    }

    public bool DeleteRefereeById(int id)
    {
      Referee referee = _context.Referees.FirstOrDefault(x => x.Id == id);
      if (referee != null)
      {
        _context.Remove(referee);
        _context.SaveChanges();
        return true;
      }
      return false;
    }
  }
}
