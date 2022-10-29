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
  public class CompanyEventRepository : ICompanyEventRepository
  {
    private readonly DomarserviceContext _context = null;
    private readonly IRefereeRepository _refereeRepository;
    private readonly IMapper _mapper;

    public CompanyEventRepository(DomarserviceContext context, IRefereeRepository refereeRepository, IMapper mapper)
    {
      _context = context;
      _refereeRepository = refereeRepository;
      _mapper = mapper;
    }

    public async Task<CompanyEventDto> GetCompanyEventById(int id)
    {
      CompanyEvent companyEvent = await _context.CompanyEvents
        .Include(x => x.RefereeTypesForEvent)
        .Include(x => x.BookingRequestByReferees)
          .ThenInclude(y => y.Referee)
        .FirstOrDefaultAsync(x => x.Id == id);

      var mappedEvent = _mapper.Map<CompanyEventDto>(companyEvent);

      // Add user info to mapped referee.
      foreach (var bookingRequest in mappedEvent.BookingRequestByReferees)
      {
        bookingRequest.Referee = await _refereeRepository.GetSimpleRefeereById(bookingRequest.Referee.Id);
      }

      return mappedEvent;
    }

    public async Task<List<ExtendedCompanyEventDto>> GetLatestCompanyEvents(int amount)
    {
      List<CompanyEvent> companyEvents = await _context.CompanyEvents
        .Include(x => x.Company)
        .Include(x => x.RefereeTypesForEvent)
        .Include(x => x.BookingRequestByReferees)
          .ThenInclude(y => y.Referee)
        // Omit dates that has allready passed.
        .Where(x => x.Date > DateTime.UtcNow)
        .Take(amount)
        .OrderBy(x => x.Id)
        .ToListAsync();
      return _mapper.Map<List<ExtendedCompanyEventDto>>(companyEvents);
    }

    public async Task<List<ExtendedCompanyEventDto>> EventsForReferee(int refereeId)
    {
      List<CompanyEvent> companyEvents = await _context.CompanyEvents
        .Include(x => x.Company)
        .Include(x => x.RefereeTypesForEvent)
        .Include(x => x.BookingRequestByReferees)
          .ThenInclude(y => y.Referee)
        // Omit dates that has allready passed.
        .Where(x => x.Date > DateTime.UtcNow)
        .Where(x => x.BookingRequestByReferees
          .Any(y => y.RefereeId == refereeId)
        )
        .OrderBy(x => x.Id)
        .ToListAsync();
      return _mapper.Map<List<ExtendedCompanyEventDto>>(companyEvents);
    }

    public async Task<List<CompanyEventDto>> GetMyEvents(int companyId)
    {
      List<CompanyEvent> companyEvents = await _context.CompanyEvents
        .Include(x => x.Company)
        .Include(x => x.RefereeTypesForEvent)
        .Include(x => x.BookingRequestByReferees)
          .ThenInclude(y => y.Referee)
        // Omit dates that has allready passed.
        .Where(x => x.Date >= DateTime.UtcNow)
        .Where(x => x.CompanyId == companyId)
        .ToListAsync();

      var mappedEvents = _mapper.Map<List<CompanyEventDto>>(companyEvents);

      // Add user info to mapped referee for each event.
      foreach (var currentEvent in mappedEvents)
      {
        foreach (var bookingRequest in currentEvent.BookingRequestByReferees)
        {
          bookingRequest.Referee = await _refereeRepository.GetSimpleRefeereById(bookingRequest.Referee.Id);
        }
      }
      
      return mappedEvents;
    }

    public async Task<List<ExtendedCompanyEventDto>> GetAllEventsPage(int page)
    {
      int maxAmount = 4;
      List<CompanyEvent> companyEvents = await _context.CompanyEvents
        .Include(x => x.Company)
        .Include(x => x.RefereeTypesForEvent)
        .Include(x => x.BookingRequestByReferees)
          .ThenInclude(y => y.Referee)
        .Skip((page - 1) * maxAmount)
        // Omit dates that has allready passed.
        .OrderBy(x => x.Date)
        .Where(x => x.Date > DateTime.UtcNow)
        .Take(maxAmount)
        .ToListAsync();
      return _mapper.Map<List<ExtendedCompanyEventDto>>(companyEvents);
    }

    public async Task<List<ExtendedCompanyEventDto>> GetFilteredEventsPage(CompanyEventsFiltered model)
    {
      int maxAmount = 20;
      List<CompanyEvent> companyEvents = await _context.CompanyEvents
        .Include(x => x.Company)
        .Include(x => x.RefereeTypesForEvent)
        .Include(x => x.BookingRequestByReferees)
          .ThenInclude(y => y.Referee)
        .WhereIf(model.FromDate <= DateTime.UtcNow, x => x.Date > DateTime.UtcNow)
        .WhereIf(model.FromDate > DateTime.UtcNow, x => x.Date > model.FromDate.ToUniversalTime())
        .WhereIf(model.SportsFilter.Length > 0, x => model.SportsFilter.Contains(x.SportType))
        .WhereIf(model.CountysFilter.Length > 0, x => model.CountysFilter.Contains(x.Company.County))
        .WhereIf(model.RefereesFilter.Length > 0, x => x.RefereeTypesForEvent.Any(referee => model.RefereesFilter.Contains(referee.RefereeType)))
        .WhereIf(model.CompanySearchString != "", x => x.Company.Name.ToLower().Contains(model.CompanySearchString.ToLower()))
        .OrderBy(x => x.Date)
        .Skip((model.Page - 1) * maxAmount)
        .Take(maxAmount)
        .ToListAsync();

      return _mapper.Map<List<ExtendedCompanyEventDto>>(companyEvents);
    }

    public async Task<bool> AddCompanyEvent(CreateCompanyEventBody request, int companyId)
    {
      try
      {
        await _context.CompanyEvents
          .AddAsync(new CompanyEvent()
          {
            CompanyId = companyId,
            Date = request.Date.ToUniversalTime(),
            Location = request.Location,
            Name = request.Name,
            SportType = request.SportType,
            RefereeTypesForEvent = request.RefereeTypesForEvent
          });
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> DeleteCompanyEventById(int id)
    {
      // TODO: Also needs to unset the RequestingCompanyEventId key to 0 on BookingRequestByCompany if there is any.

      CompanyEvent companyEvent = await _context.CompanyEvents.FirstOrDefaultAsync(x => x.Id == id);
      var companyEventId = companyEvent.Id;
      if (companyEvent != null)
      {
        // Also set requestingCompanyEventId to 0 if any BookingRequestByCompanys has this id.
        // So that it doesnt refer to a non existing companyevent.
        List<BookingRequestByCompany> companyRequests = await _context.BookingRequestsByCompany.Where(x => x.RequestingCompanyEventId == companyEventId).ToListAsync();
        foreach (var r in companyRequests)
        {
          r.RequestingCompanyEventId = 0;
        }

        _context.Remove(companyEvent);
        await _context.SaveChangesAsync();
        return true;
      }
      return false;
    }
  }
}
