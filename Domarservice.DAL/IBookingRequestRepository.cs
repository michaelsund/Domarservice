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
        Task<List<BookingRequestByCompanyDto>> GetCompanysRequestOnRefereeSchedule(int companyId);
        Task<bool> AddBookingRequestByCompany(BookScheduleByCompanyBody request, int companyId);
        Task<bool> AwnserBookingRequestFromCompany(AwnserCompanyRequestBody request, int refereeId);
        // Referee > Company
        Task<ResultWithMessage> AddBookingRequestByReferee(BookCompanyEventByRefereeBody request, int refereeId);
        Task<ResultWithMessage> RemoveBookingRequestByReferee(int requestId, int refereeId);
        Task<ResultWithMessage> RemoveBookingRequestOnRefereeSchedule(int requestId, int companyId);
        Task<bool> AwnserBookingRequestFromReferee(AwnserRefereeRequestBody request, int companyId);
        Task<bool> GetBookingRequestByDateForReferee(DateTime scheduleFrom, DateTime scheduleTo, int refereeId);
    }
}
