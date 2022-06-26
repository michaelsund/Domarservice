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

    public async Task<RefereeDto> GetRefeereById(int id)
    {
      Referee referee = await _context.Referees
     .Include(x => x.Sports)
     .Include(x => x.Countys)
     .Include(x => x.Schedules)
       .ThenInclude(y => y.BookingRequestByCompanys)
       .ThenInclude(y => y.RequestingCompany)
     .FirstOrDefaultAsync(x => x.Id == id);
      return _mapper.Map<RefereeDto>(referee);
    }

    public async Task<bool> DeleteRefereeById(int? id)
    {
      if (id != null)
      {
         Referee referee = await _context.Referees.FirstOrDefaultAsync(x => x.Id == id);
        if (referee != null)
        {
          _context.Remove(referee);
          _context.SaveChanges();
          return true;
        }
      }
      return false;
    }
  }
}
