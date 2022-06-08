using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.Models;
using Domarservice.Helpers;

namespace Domarservice.DAL
{
    public interface IBookingRequestRepository
    {
        Task<bool> AddBookingRequestByCompany(BookScheduleByCompanyBody request);
        Task<bool> AwnserBookingRequestFromCompany(AwnserCompanyRequestBody request, int refereeId);
    }
}
