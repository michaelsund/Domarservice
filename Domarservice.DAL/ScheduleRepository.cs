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

    public ScheduleDto GetScheduleById(int id)
    {
      Schedule schedule = _context.Schedules
        .Include(x => x.Referee)
        .Include(x => x.BookingRequestByCompanys)
          .ThenInclude(y => y.RequestingCompany)
        .FirstOrDefault(x => x.Id == id);
      var model = _mapper.Map<ScheduleDto>(schedule);

      return model;
    }

    public bool DeleteScheduleById(int id)
    {
      Schedule schedule = _context.Schedules.FirstOrDefault(x => x.Id == id);
      if (schedule != null)
      {
        _context.Remove(schedule);
        _context.SaveChanges();
        return true;
      }
      return false;
    }
  }
}
