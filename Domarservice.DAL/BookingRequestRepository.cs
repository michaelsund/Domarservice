using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Domarservice.Helpers;
using AutoMapper;
using Domarservice.Models;

namespace Domarservice.DAL
{
  // OBS! This repo handles both company and referee booking requests.
  public class BookingRequestRepository : IBookingRequestRepository
  {
    private readonly DomarserviceContext _context = null;
    private readonly IMapper _mapper;
    private readonly IRefereeRepository _refereeRepository;

    public BookingRequestRepository(DomarserviceContext context, IMapper mapper, IRefereeRepository refereeRepository)
    {
      _context = context;
      _mapper = mapper;
      _refereeRepository = refereeRepository;
    }

    public async Task<List<BookingRequestByCompanyDto>> GetCompanysRequestOnRefereeSchedule(int companyId)
    {
      try
      {
        List<BookingRequestByCompany> requests = await _context.BookingRequestsByCompany
          .Include(x => x.Schedule)
            .ThenInclude(y => y.Referee)
          .Where(x => x.Schedule.From >= DateTime.Now.Date)
          .Where(x => x.RequestingCompany.Id == companyId)
          .ToListAsync();
        var mappedBookingRequests = _mapper.Map<List<BookingRequestByCompanyDto>>(requests);
        foreach (var item in mappedBookingRequests)
        {
          var simpleReferee = await _refereeRepository.GetSimpleRefeereById(item.Schedule.RefereeId);
          item.Schedule.Referee = _mapper.Map<SimpleRefereeDto>(simpleReferee);
        }
        return mappedBookingRequests;
      }
      catch (Exception)
      {
        return new List<BookingRequestByCompanyDto>();
      }
    }

    public async Task<bool> AddBookingRequestByCompany(BookScheduleByCompanyBody request, int companyId)
    {
      try
      {
        // First check if the company allready has a bookingrequest on this schedule id.

        var requests = await _context.BookingRequestsByCompany
          .Where(x => x.ScheduleId == request.ScheduleId)
          .ToListAsync();

        if (requests.Any(x => x.CompanyId == companyId))
        {
          // This company allready has a request on this scheduleId
          return false;
        }

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

    public async Task<bool> GetBookingRequestByDateForReferee(DateTime scheduleFrom, DateTime scheduleTo, int refereeId)
    {
      try
      {
        List<BookingRequestByReferee> bookings = await _context.BookingRequestsByReferee
          .Include(x => x.CompanyEvent)
          .Where(x => x.RefereeId == refereeId)
          .ToListAsync();
        if (bookings.Count > 0)
        {
          foreach (var booking in bookings)
          {
            var start = booking.CompanyEvent.Date;
            var end = booking.CompanyEvent.Date.AddHours(booking.CompanyEvent.DurationHours).AddMinutes(booking.CompanyEvent.DurationMinutes);

            // Detect collission otherwise return no bookings collide with the new schedule
            if (scheduleFrom <= start && scheduleTo >= end)
            {              
              return true;
            }
            else if (scheduleFrom >= start && scheduleTo <= end)
            {
              return true;
            }
          }
          return false;
        }
        return false;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<ResultWithMessage> AddBookingRequestByReferee(BookCompanyEventByRefereeBody request, int refereeId)
    {
      // First check if the companyEvent exists.
      try
      {
        var companyEvent = await _context.CompanyEvents
          .Include(x => x.BookingRequestByReferees)
          .Include(x => x.RefereeTypesForEvent)
          .FirstOrDefaultAsync(x => x.Id == request.CompanyEventId);
        if (companyEvent != null)
        {
          // Check that the sports matches up, fetch the referee and check the sports
          var referee = await _context.Referees
            .Include(x => x.Sports)
            .FirstOrDefaultAsync(x => x.Id == refereeId);
          var sportsTypesList = new List<SportType>();
          foreach (var sport in referee.Sports)
          {
            sportsTypesList.Add(sport.SportType);
          }

          if (sportsTypesList.Contains(companyEvent.SportType))
          {
            // Is all referee slots filled?
            var resultWithMessage = new RefereeTypeQuotaForEvent().Check(companyEvent, request);

            if (!resultWithMessage.Result)
            {
              // This should return a better errorMessage saying that the role is allready filled.
              return resultWithMessage;
            }

            // Check that the referee has not allready made a request.
            var exists = await _context.BookingRequestsByReferee.Where(x =>
              x.CompanyEventId == request.CompanyEventId &&
              x.RefereeId == refereeId &&
              x.RefereeType == request.RefereeType
            ).FirstOrDefaultAsync();

            if (exists != null)
            {
              return new ResultWithMessage
              {
                Result = false,
                Message = "Du har redan ansökt om att döma.",
                Data = null
              };
            }

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
            return new ResultWithMessage
            {
              Result = true,
              Message = "Din ansökan är skickad.",
              Data = null
            };
          }
        }
        return new ResultWithMessage
        {
          Result = false,
          Message = "Kunde inte hitta matchen.",
          Data = null
        };
      }
      catch (Exception e)
      {
        return new ResultWithMessage
        {
          Result = false,
          Message = "Ett fel uppstod när ansökan skickades.",
          Data = null
        };
      }
    }

    public async Task<ResultWithMessage> RemoveBookingRequestOnRefereeSchedule(int requestId, int companyId)
    {
      try
      {
        var request = await _context.BookingRequestsByCompany.Where(
          x => x.Id == requestId && x.CompanyId == companyId
        ).FirstOrDefaultAsync();
        if (request != null)
        {
          // Do not allow companies to revoke if the referee have accepted.
          if (request.Accepted)
          {
            return new ResultWithMessage
            {
              Result = false,
              Message = "Din schemabokning kan inte tas bort när den accepterats.",
              Data = null
            };
          }

          _context.BookingRequestsByCompany.Remove(request);
          var result = await _context.SaveChangesAsync();
          if (result > 0)
          {
            return new ResultWithMessage
            {
              Result = true,
              Message = "Din schemabokning är nu borttagen.",
              Data = null
            };
          }
        }
        return new ResultWithMessage
        {
          Result = false,
          Message = "Kunde inte hitta schemabokningen.",
          Data = null
        }; ;
      }
      catch (Exception)
      {
        return new ResultWithMessage
        {
          Result = true,
          Message = "Ett problem uppstod när schemabokningen skulle tas bort.",
          Data = null
        };
      }
    }

    public async Task<ResultWithMessage> RemoveBookingRequestByReferee(int requestId, int refereeId)
    {
      try
      {
        var request = await _context.BookingRequestsByReferee.Where(x =>
           x.CompanyEventId == requestId &&
           x.RefereeId == refereeId
         ).FirstOrDefaultAsync();
        if (request != null)
        {
          // Do not allow referees to revoke if they have been accepted.
          if (request.Accepted)
          {
            return new ResultWithMessage
            {
              Result = false,
              Message = "Din förfrågan kan inte tas bort när den accepterats.",
              Data = null
            };
          }

          _context.BookingRequestsByReferee.Remove(request);
          var result = await _context.SaveChangesAsync();
          if (result > 0)
          {
            return new ResultWithMessage
            {
              Result = true,
              Message = "Din förfrågan är nu borttagen.",
              Data = null
            };
          }
        }
        return new ResultWithMessage
        {
          Result = false,
          Message = "Kunde inte hitta din förfrågan.",
          Data = null
        }; ;
      }
      catch (Exception)
      {
        return new ResultWithMessage
        {
          Result = true,
          Message = "Ett problem uppstod när din förfrågan skulle tas bort.",
          Data = null
        };
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
