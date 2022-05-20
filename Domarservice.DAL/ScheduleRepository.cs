using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Domarservice.Models;
using AutoMapper;

namespace Domarservice.DAL
{
  public class ScheduleRepository : IScheduleRepository
  {
    private readonly DomarserviceContext _context = null;
    private readonly IMapper _mapper;

    public ScheduleRepository(DomarserviceContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<ScheduleDto> GetScheduleById(int id)
    {
      try
      {
        Schedule schedule = await _context.Schedules
          .Include(x => x.Referee)
          .Include(x => x.BookingRequestByCompanys)
            .ThenInclude(y => y.RequestingCompany)
          .FirstOrDefaultAsync(x => x.Id == id);
        return _mapper.Map<ScheduleDto>(schedule);
      }
      catch (Exception ex)
      {
        throw ex;
      }

    }

    public async Task<bool> DeleteScheduleById(int id)
    {
      Schedule schedule = await _context.Schedules.FirstOrDefaultAsync(x => x.Id == id);
      if (schedule != null)
      {
        _context.Remove(schedule);
        await _context.SaveChangesAsync();
        return true;
      }
      return false;
    }
  }
}
