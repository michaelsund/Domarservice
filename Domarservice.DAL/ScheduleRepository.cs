using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Domarservice.Models;
using Domarservice.Helpers;
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
      Schedule schedule = await _context.Schedules
        .Include(x => x.Referee)
        .Include(x => x.BookingRequestByCompanys)
          .ThenInclude(y => y.RequestingCompany)
        .FirstOrDefaultAsync(x => x.Id == id);
      return _mapper.Map<ScheduleDto>(schedule);
    }

    public async Task<List<SimpleScheduleDto>> GetSchedulesByRefereeId(int id)
    {
      List<Schedule> schedules = await _context.Schedules
        .Include(x => x.BookingRequestByCompanys)
          .ThenInclude(y => y.RequestingCompany)
        .Where(x => x.RefereeId == id)
        .ToListAsync();
      return _mapper.Map<List<SimpleScheduleDto>>(schedules);
    }

    public async Task<bool> CreateSchedule(int id, DateTime availableAt)
    {
      try
      {
        await _context.Schedules
        .AddAsync(new Schedule()
        {
          AvailableAt = availableAt,
          RefereeId = id,
        });
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception)
      {
        return false;
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
