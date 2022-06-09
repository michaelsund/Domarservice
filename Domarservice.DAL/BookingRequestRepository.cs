using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Domarservice.Helpers;
using AutoMapper;

namespace Domarservice.DAL
{
  // OBS! This repo handles both company and referee booking requests.
  public class BookingRequestRepository : IBookingRequestRepository
  {
    private readonly DomarserviceContext _context = null;
    private readonly IMapper _mapper;

    public BookingRequestRepository(DomarserviceContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<bool> AddBookingRequestByCompany(BookScheduleByCompanyBody request, int companyId)
    {
      try
      {
        await _context.BookingRequestsByCompany
        .AddAsync(new BookingRequestByCompany()
        {
          Accepted = false,
          CompanyId = companyId,
          // Set to 0 if there is not a companyEvent specified.
          RequestingCompanyEventId = request.CompanyEventId | 0,
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

    public async Task<bool> AwnserBookingRequestFromCompany(AwnserCompanyRequestBody request, int refereeId)
    {
      try
      {
        var bookingRequest = await _context.BookingRequestsByCompany
          .FirstOrDefaultAsync(x => x.Id == request.RequestId);

        if (bookingRequest != null)
        {
          // Check if the referee id is allowed to accept this request, by looking up the refereeId from the schedule
          var schedule = await _context.Schedules.FirstOrDefaultAsync(x => x.Id == bookingRequest.ScheduleId);
          if (schedule.RefereeId == refereeId)
          {
            bookingRequest.Accepted = request.Accepted;
            bookingRequest.RespondedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
          }
        }
        return false;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> AddBookingRequestByReferee(BookCompanyEventByRefereeBody request, int refereeId)
    {
      // First check if the companyEvent exists.
      try
      {
        var companyEvent = await _context.CompanyEvents.FirstOrDefaultAsync(x => x.Id == request.CompanyEventId);
        if (companyEvent != null)
        {
          await _context.BookingRequestsByReferee
            .AddAsync(new BookingRequestByReferee()
            {
              Accepted = false,
              RefereeId = refereeId,
              Message = request.Message,
              CompanyEventId = request.CompanyEventId,
              AppliedAt = DateTime.UtcNow,
              RefereeType = request.RefereeType
            });
          await _context.SaveChangesAsync();
          return true;
        }
        return false;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> AwnserBookingRequestFromReferee(AwnserRefereeRequestBody request, int companyId)
    {
      try
      {
        var bookingRequest = await _context.BookingRequestsByReferee
          .Include(x => x.CompanyEvent)
          .FirstOrDefaultAsync(x => x.Id == request.RequestId);
        
        if (bookingRequest != null && bookingRequest.CompanyEvent.CompanyId == companyId)
        {
            bookingRequest.Accepted = request.Accepted;
            bookingRequest.RespondedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
