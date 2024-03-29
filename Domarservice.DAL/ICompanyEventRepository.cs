﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.Models;
using Domarservice.Helpers;

namespace Domarservice.DAL
{
    public interface ICompanyEventRepository
    {
        Task<CompanyEventDto> GetCompanyEventById(int id);
        Task<List<ExtendedCompanyEventDto>> GetLatestCompanyEvents(int amount);
        Task<List<ExtendedCompanyEventDto>> EventsForReferee(int refereeId);
        Task<List<CompanyEventDto>> GetMyEvents(int companyId);
        Task<List<ExtendedCompanyEventDto>> GetAllEventsPage(int page);
        Task<List<ExtendedCompanyEventDto>> GetFilteredEventsPage(CompanyEventsFiltered model);
        Task<bool> DeleteCompanyEventById(int id);
        Task<bool> AddCompanyEvent(CreateCompanyEventBody request, int companyId);
    }
}
