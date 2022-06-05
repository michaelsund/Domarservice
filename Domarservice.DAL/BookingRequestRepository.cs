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
  public class BookingRequestRepository : IBookingRequestRepository
  {
    private readonly DomarserviceContext _context = null;
    private readonly IMapper _mapper;

    public BookingRequestRepository(DomarserviceContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<bool> AddBookingRequestByCompany(BookScheduleByCompanyBody request)
    {
      try
      {
        await _context.BookingRequestsByCompany
        .AddAsync(new BookingRequestByCompany()
        {
          Accepted = false,
          CompanyId = request.CompanyId,
          RequestingCompanyEventId = request.CompanyEventId,
          Message = request.Message,
          RefereeType = request.RefereeType,
          SportType = request.SportType,
          ScheduleId = request.ScheduleId
        });
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
    
  }
}
