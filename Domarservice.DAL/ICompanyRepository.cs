using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.Models;

namespace Domarservice.DAL
{
  public interface ICompanyRepository
  {
    Task<SimpleCompanyDto> GetSimpleCompanyById(int id);
    Task<CompanyDto> GetCompanyById(int id);
    // List<Company> GetAllCompanies();
    Task<bool> DeleteCompanyById(int id);
  }
}
