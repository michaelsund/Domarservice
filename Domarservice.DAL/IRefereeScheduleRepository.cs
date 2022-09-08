using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.Models;
using Domarservice.Helpers;

namespace Domarservice.DAL
{
    public interface IRefereeScheduleRepository
    {
        Task<List<SimpleScheduleDto>> GetSchedulesByRefereeId(int id);
        Task<RefereeScheduleDto> GetScheduleById(int id);
        Task<List<RefereeScheduleDto>> GetFilteredSchedulesPage(RefereeSchedulesFiltered model);
        Task<bool> DeleteScheduleById(int id);
        Task<bool> CreateSchedule(int id, DateTime availableAt);
    }
}
