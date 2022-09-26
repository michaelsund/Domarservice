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
        // Company > Referee
        Task<bool> AddBookingRequestByCompany(BookScheduleByCompanyBody request, int companyId);
        Task<bool> AwnserBookingRequestFromCompany(AwnserCompanyRequestBody request, int refereeId);
        // Referee > Company
        Task<bool> AddBookingRequestByReferee(BookCompanyEventByRefereeBody request, int refereeId);
        Task<bool> RemoveBookingRequestByReferee(int requestId, int refereeId);
        Task<bool> AwnserBookingRequestFromReferee(AwnserRefereeRequestBody request, int companyId);
        
    }
}
