using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.Models;

namespace Domarservice.DAL
{
    public interface ICompanyEventRepository
    {
        Task<CompanyEventDto> GetCompanyEventById(int id);
        Task<bool> DeleteCompanyEventById(int id);
    }
}
