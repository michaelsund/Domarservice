using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Domarservice.Models;
using AutoMapper;

namespace Domarservice.DAL
{
  public class CompanyRepository : ICompanyRepository
  {
    private readonly DomarserviceContext _context = null;
    private readonly IMapper _mapper;

    public CompanyRepository(DomarserviceContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public async Task<SimpleCompanyDto> GetSimpleCompanyById(int id)
    {
      Company company = await _context.Companies
     .Include(x => x.Sports)
     .FirstOrDefaultAsync(x => x.Id == id);
      return _mapper.Map<SimpleCompanyDto>(company);
    }

    public async Task<CompanyDto> GetCompanyById(int id)
    {
      Company company = await _context.Companies
     .Include(x => x.Sports)
     .Include(x => x.CompanyEvents)
       .ThenInclude(y => y.BookingRequestByReferees)
     .FirstOrDefaultAsync(x => x.Id == id);
      return _mapper.Map<CompanyDto>(company);

    }

    public async Task<bool> DeleteCompanyById(int id)
    {

      Company company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == id);
      if (company != null)
      {
        _context.Remove(company);
        await _context.SaveChangesAsync();
        return true;
      }
      return false;

    }
  }
}
