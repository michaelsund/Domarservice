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

    // public List<Company> GetallCompanies()
    // {
    //   var companies = _context.Companies.ToList();
    //   var model = _mapper.Map<CompanyDto>(companies);
    //   return model;
    // }

    public CompanyDto GetCompanyById(int id)
    {
      Company company = _context.Companies
        .Include(x => x.Sports)
        .FirstOrDefault(x => x.Id == id);
      var model = _mapper.Map<CompanyDto>(company);

      return model;
    }

    public bool DeleteCompanyById(int id)
    {
      Company company = _context.Companies.FirstOrDefault(x => x.Id == id);
      if (company != null)
      {
        _context.Remove(company);
        _context.SaveChanges();
        return true;
      }
      return false;
    }
  }
}
