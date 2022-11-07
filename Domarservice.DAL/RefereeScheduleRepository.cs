using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Domarservice.Models;
using Domarservice.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace Domarservice.DAL
{
  public class RefereeScheduleRepository : IRefereeScheduleRepository
  {
    private readonly DomarserviceContext _context = null;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public RefereeScheduleRepository(DomarserviceContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
      _context = context;
      _userManager = userManager;
      _mapper = mapper;
    }

    public async Task<RefereeScheduleDto> GetScheduleById(int id)
    {
      Schedule schedule = await _context.Schedules
        .Include(x => x.Referee)
        .Include(x => x.BookingRequestByCompanys)
          .ThenInclude(y => y.RequestingCompany)
          .ThenInclude(i => i.Sports)
        .FirstOrDefaultAsync(x => x.Id == id);
      return _mapper.Map<RefereeScheduleDto>(schedule);
    }

    public async Task<List<RefereeMonthScheduleDto>> GetScheduleByIdAndMonth(int id, int year, int month)
    {
      List<Schedule> schedule = await _context.Schedules
        .Include(x => x.BookingRequestByCompanys)
          .ThenInclude(y => y.RequestingCompany)
          .ThenInclude(i => i.Sports)
        .Where(x => x.RefereeId == id)
        .Where(x => x.AvailableAt.Year == year)
        .Where(x => x.AvailableAt.Month == month)
        .ToListAsync();
      var availableDays = _mapper.Map<List<RefereeScheduleDto>>(schedule);

      // Generate list with days for the month, and tack on the availableDays objects.
      // Need a new model with date, date number, and day name in swedish.
      CultureInfo myCI = new CultureInfo("sv-SE", false);
      var monthSchedule = new List<RefereeMonthScheduleDto>();

      foreach (var date in DateHelper.AllDaysInMonth(year, month))
      {
        bool hasMarkedAsAvailable = availableDays.Any(x => DateTime.Parse(x.AvailableAt).Day == date.Date.Day);
        var day = new RefereeMonthScheduleDto
        {
          Day = date.Day,
          DayName = myCI.DateTimeFormat.GetDayName(date.DayOfWeek),
          Week = System.Globalization.ISOWeek.GetWeekOfYear(date)
        };

        if (hasMarkedAsAvailable)
        {
          var availableDay = availableDays.Where(x => DateTime.Parse(x.AvailableAt).Day == date.Date.Day).FirstOrDefault();
          day.AvailableAt = availableDay.AvailableAt;
          day.BookingRequestByCompanys = availableDay.BookingRequestByCompanys;
        }

        monthSchedule.Add(day);
      }

      return monthSchedule;
    }

    public async Task<List<RefereeScheduleDto>> ScheduleRequestsForReferee(int refereeId)
    {
      // Only return the schedules that has requests from companies.
      List<Schedule> schedules = await _context.Schedules
        .Include(x => x.BookingRequestByCompanys)
          .ThenInclude(y => y.RequestingCompany)
          .ThenInclude(i => i.Sports)
        // Omit dates that has allready passed.
        .Where(x => x.AvailableAt > DateTime.UtcNow)
        .Where(x => x.RefereeId == refereeId)
        // Omit dates that has allready passed.
        .Where(x => x.BookingRequestByCompanys.Count > 0)
        .ToListAsync();
      var refereeSchedules = _mapper.Map<List<RefereeScheduleDto>>(schedules);
      if (refereeSchedules.Count > 0)
      {
        return refereeSchedules;
      }
      return new List<RefereeScheduleDto>();
    }

    public async Task<List<SimpleScheduleDto>> GetSchedulesByRefereeId(int id)
    {
      List<Schedule> schedules = await _context.Schedules
        .Include(x => x.BookingRequestByCompanys)
          .ThenInclude(y => y.RequestingCompany)
          .ThenInclude(i => i.Sports)
        .Where(x => x.RefereeId == id)
        .ToListAsync();
      var refereeSchedules = _mapper.Map<List<SimpleScheduleDto>>(schedules);
      if (refereeSchedules.Count > 0)
      {
        return refereeSchedules;
      }
      return null;
    }

    public async Task<List<RefereeScheduleDto>> GetFilteredSchedulesPage(RefereeSchedulesFiltered model)
    {
      int maxAmount = 20;
      List<Schedule> refereeSchedules = await _context.Schedules
        .Include(x => x.Referee)
          .ThenInclude(y => y.Sports)
        .Include(x => x.Referee)
          .ThenInclude(y => y.Countys)
        // .Include(x => x.BookingRequestByCompanys)
        // Skip passed dates
        // .WhereIf(model.AvailableFromDate < DateTime.UtcNow.AddDays(-1), x => x.AvailableAt > DateTime.UtcNow)
        .WhereIf(model.AvailableFromDate <= DateTime.UtcNow, x => x.AvailableAt > DateTime.UtcNow)
        .WhereIf(model.AvailableFromDate > DateTime.UtcNow, x => x.AvailableAt > model.AvailableFromDate.ToUniversalTime())
        .WhereIf(model.SportsFilter.Length > 0, x => x.Referee.Sports.Any(x => model.SportsFilter.Contains(x.SportType)))
        .WhereIf(model.CountysFilter.Length > 0, x => x.Referee.Countys.Any(county => model.CountysFilter.Contains(county.CountyType)))

        // Surname / Lastname search
        // .WhereIf(model.CompanySearchString != "", x => x.Company.Name.ToLower().Contains(model.CompanySearchString.ToLower()))
        .OrderBy(x => x.AvailableAt)
        .Skip((model.Page - 1) * maxAmount)
        .Take(maxAmount)
        .ToListAsync();

      var schedules = _mapper.Map<List<RefereeScheduleDto>>(refereeSchedules);

      // Fill in some refereeInfo from each user. Lastname, Surname
      foreach (var schedule in schedules)
      {
        try
        {
          schedule.Referee = await GetInfoFromUserByRefereeId(schedule.RefereeId, schedule.Referee.Sports, schedule.Referee.Countys);
        }
        catch (Exception)
        {
          schedule.Referee.Surname = "Borttagen";
          schedule.Referee.Lastname = "Borttagetsson";
        }
      }

      // Remove all schedules who's referee cannot be mapped to a user.
      schedules.RemoveAll(x => x.Referee.Surname == "Borttagen");
      return schedules;
    }

    public async Task<bool> CreateSchedule(int id, DateTime availableAt)
    {
      try
      {
        await _context.Schedules
        .AddAsync(new Schedule()
        {
          AvailableAt = availableAt,
          RefereeId = id,
        });
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> DeleteScheduleById(int id)
    {
      Schedule schedule = await _context.Schedules.FirstOrDefaultAsync(x => x.Id == id);
      if (schedule != null)
      {
        _context.Remove(schedule);
        await _context.SaveChangesAsync();
        return true;
      }
      return false;
    }

    public async Task<SimpleRefereeDto> GetInfoFromUserByRefereeId(int refereeId, List<RefereeSportDto> refereeSports, List<CountyDto> refereeCountys)
    {
      // Find the user "only one" that has the corresponding refereeId set
      var userInfo = await _context.ApplicationUsers.Where(x => x.RefereeId == refereeId).FirstOrDefaultAsync();
      var refereeDto = new SimpleRefereeDto
      {
        Id = refereeId,
        Surname = userInfo.Surname,
        Lastname = userInfo.Lastname,
        Sports = refereeSports,
        Countys = refereeCountys,
      };

      return refereeDto;
    }
  }
}
