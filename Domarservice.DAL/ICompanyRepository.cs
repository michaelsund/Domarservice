using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.Models;
using Domarservice.Helpers;

namespace Domarservice.DAL
{
  public interface ICompanyRepository
  {
    Task<SimpleCompanyDto> GetSimpleCompanyById(int id);
    Task<CompanyAndUsersDto> GetCompanyUsersByCompanyId(int id);
    Task<CompanyDto> GetCompanyById(int id);
    Task<Company> AddNewCompany(RegisterCompanyModel model); 
    Task<bool> DeleteCompanyById(int id);
  }
}
