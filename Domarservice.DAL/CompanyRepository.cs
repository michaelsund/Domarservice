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

    public async Task<List<SimpleUserDto>> GetCompanyUsersByCompanyId(int id)
    {
      List<ApplicationUser> companyUsers = await _context.ApplicationUsers.Where(x => x.CompanyId == id)
        .ToListAsync();
    //   Company company = await _context.Companies
    //  .Include(x => x.Sports)
    //  .FirstOrDefaultAsync(x => x.Id == id);
      return _mapper.Map<List<SimpleUserDto>>(companyUsers);
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

    public async Task<bool> AddNewCompany(RegisterCompanyModel model)
    {

      await _context.Companies.AddAsync(new Company()
      {
        Name = model.Name,
        Address = model.Address,
        County = model.County,
        City = model.City,
        Email = model.Email,
        PhoneOne = model.PhoneOne,
        PhoneTwo = model.PhoneTwo,
        Sports = model.Sports,
        HasValidSubscription = false,
      });
      var result = await _context.SaveChangesAsync();
      if (result > 0)
      {
        return true;
      }
      return false;
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
