using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Domarservice.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Domarservice.DAL
{
  public class RefereeRepository : IRefereeRepository
  {
    private readonly DomarserviceContext _context = null;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public RefereeRepository(DomarserviceContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _mapper = mapper;
      _userManager = userManager;
    }

    public async Task<RefereeDto> GetRefeereById(int id)
    {
      // Get the User for this referee Id
      ApplicationUser user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.RefereeId == id);
      if (user != null)
      {
        // Then map the user via RefereeId to the referee object
        Referee referee = await _context.Referees
       .Include(x => x.Sports)
       .Include(x => x.Countys)
       .Include(x => x.Schedules)
         .ThenInclude(y => y.BookingRequestByCompanys)
         .ThenInclude(y => y.RequestingCompany)
       .FirstOrDefaultAsync(x => x.Id == id);
        var mappedReferee = _mapper.Map<RefereeDto>(referee);
        mappedReferee.Surname = user.Surname;
        mappedReferee.Lastname = user.Lastname;
        return mappedReferee;
      }
      return null;
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
