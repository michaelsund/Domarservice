using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Domarservice.Models;
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
      try
      {
        CompanyEvent companyEvent = await _context.CompanyEvents
          .Include(x => x.BookingRequestByReferees)
          .FirstOrDefaultAsync(x => x.Id == id);
        return _mapper.Map<CompanyEventDto>(companyEvent);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public async Task<bool> DeleteCompanyEventById(int id)
    {
      // TODO: Also needs to unset the RequestingCompanyEventId key to 0 on BookingRequestByCompany if there is any.
      try
      {
        CompanyEvent companyEvent = await _context.CompanyEvents.FirstOrDefaultAsync(x => x.Id == id);
        if (companyEvent != null)
        {
          _context.Remove(companyEvent);
          await _context.SaveChangesAsync();
          return true;
        }
        return false;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }
  }
}
