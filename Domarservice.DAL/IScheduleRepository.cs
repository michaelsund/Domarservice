using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.Models;
using Domarservice.Helpers;

namespace Domarservice.DAL
{
    public interface IScheduleRepository
    {
        Task<List<SimpleScheduleDto>> GetSchedulesByRefereeId(int id);
        Task<ScheduleDto> GetScheduleById(int id);
        Task<bool> DeleteScheduleById(int id);
        Task<bool> CreateSchedule(int id, DateTime availableAt);
    }
}
