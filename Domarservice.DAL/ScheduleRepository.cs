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
        .Include(x => x.ClaimedByCompany)
          .ThenInclude(x => x.Sports)
        .Include(x => x.BookingRequests)
        .FirstOrDefault(x => x.Id == id);
      var model = _mapper.Map<ScheduleDto>(schedule);

      return model;
    }
  }
}
