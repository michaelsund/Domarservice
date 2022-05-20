using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.Models;

namespace Domarservice.DAL
{
    public interface IScheduleRepository
    {
        Task<ScheduleDto> GetScheduleById(int id);
        Task<bool> DeleteScheduleById(int id);
    }
}
