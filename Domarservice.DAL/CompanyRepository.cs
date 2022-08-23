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

    public async Task<CompanyAndUsersDto> GetCompanyUsersByCompanyId(int id)
    {
      List<ApplicationUser> companyUsers = await _context.ApplicationUsers.Where(x => x.CompanyId == id)
        .ToListAsync();
      var mappedUsers = _mapper.Map<List<SimpleUserDto>>(companyUsers);

      Company company = await _context.Companies
     .Include(x => x.Sports)
     .FirstOrDefaultAsync(x => x.Id == id);
      var mappedCompany = _mapper.Map<CompanyAndUsersDto>(company);
      mappedCompany.Users = mappedUsers;
      return mappedCompany;
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

    public bool CompanyUniqueByName(string name)
    {
      try
      {
        var company = _context.Companies.FirstOrDefault(x => x.Name == name);
        if (company != null)
        {
          return false;
        }
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<Company> AddNewCompany(RegisterCompanyModel model)
    {
      var company = new Company()
      {
        Name = model.Name,
        City = model.City,
        County = model.County,
        HasValidSubscription = false,
      };
      await _context.Companies.AddAsync(company);
      await _context.SaveChangesAsync();

      return company;
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
