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
    private readonly IMapper _mapper;

    public CompanyEventRepository(DomarserviceContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<CompanyEventDto> GetCompanyEventById(int id)
    {

      CompanyEvent companyEvent = await _context.CompanyEvents
        .Include(x => x.RefereeTypesForEvent)
        .Include(x => x.BookingRequestByReferees)
          .ThenInclude(y => y.Referee)
        .FirstOrDefaultAsync(x => x.Id == id);
      return _mapper.Map<CompanyEventDto>(companyEvent);
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
        .OrderBy(x => x.Date)
        .ToListAsync();
      return _mapper.Map<List<ExtendedCompanyEventDto>>(companyEvents);
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

    public async Task<bool> AddCompanyEvent(CreateCompanyEventBody request, int companyId)
    {
      try
      {
        await _context.CompanyEvents
          .AddAsync(new CompanyEvent()
          {
            CompanyId = companyId,
            Date = request.Date,
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
