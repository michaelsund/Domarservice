using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.Models;

namespace Domarservice.DAL
{
  public interface ICompanyRepository
  {
    CompanyDto GetCompanyById(int id);
    // List<Company> GetAllCompanies();
    bool DeleteCompanyById(int id);
  }
}
