using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Domarservice.DAL;
using Domarservice.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace Domarservice.BLL
{
  public class DoTest
  {
    private readonly DomarserviceContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public DoTest(
      DomarserviceContext context,
      UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager
    )
    {
      _context = context;
      _userManager = userManager;
      _roleManager = roleManager;
    }
    public async Task SetupRoles()
    {
      if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
      if (!await _roleManager.RoleExistsAsync(UserRoles.RefereeUser))
        await _roleManager.CreateAsync(new IdentityRole(UserRoles.RefereeUser));
      if (!await _roleManager.RoleExistsAsync(UserRoles.CompanyUser))
        await _roleManager.CreateAsync(new IdentityRole(UserRoles.CompanyUser));
    }
    public void AddReferee(string surname, string lastname, int i)
    {
      _context.Referees.Add(new Referee()
      {
        Lastname = lastname,
        Surname = surname,
        Sports = new List<RefereeSport>() {
          new RefereeSport() { SportType = SportType.Fotboll, RefereeType = RefereeType.Hudvuddomare },
          new RefereeSport() { SportType = SportType.Ishockey, RefereeType = RefereeType.Linjeman },
        }
      });
      _context.SaveChanges();
    }
    public async Task AddFirstAdminUser()
    {
      ApplicationUser user = new()
      {
        Email = "admin@osund.com",
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = "admin@osund.com"
      };
      user.EmailConfirmed = true;
      await _userManager.CreateAsync(user, "!Oneverycomplexpassword123");

      if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
      if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
      {
        await _userManager.AddToRoleAsync(user, UserRoles.Admin);
      }
    }

    public async Task AddFirstRefereeUser()
    {
      ApplicationUser user = new()
      {
        Email = "michael@osund.com",
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = "michael@osund.com"
      };
      user.EmailConfirmed = true;
      // Seed data user 1 has referee 1
      user.RefereeId = 1;
      await _userManager.CreateAsync(user, "!Oneverycomplexpassword123");

      if (!await _roleManager.RoleExistsAsync(UserRoles.RefereeUser))
        await _roleManager.CreateAsync(new IdentityRole(UserRoles.RefereeUser));
      if (await _roleManager.RoleExistsAsync(UserRoles.RefereeUser))
      {
        await _userManager.AddToRoleAsync(user, UserRoles.RefereeUser);
      }
    }

    public async Task AddFirstCompanyUser()
    {
      ApplicationUser user = new()
      {
        Email = "michael2@osund.com",
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = "michael2@osund.com"
      };
      user.EmailConfirmed = true;
      // Seed data user 1 has referee 1
      user.CompanyId = 1;
      await _userManager.CreateAsync(user, "!Oneverycomplexpassword123");

      if (!await _roleManager.RoleExistsAsync(UserRoles.CompanyUser))
        await _roleManager.CreateAsync(new IdentityRole(UserRoles.CompanyUser));
      if (await _roleManager.RoleExistsAsync(UserRoles.CompanyUser))
      {
        await _userManager.AddToRoleAsync(user, UserRoles.CompanyUser);
      }
    }

    public void AddScheduleDate(int id)
    {
      Referee referee = _context.Referees.Where(x => x.Id == id).FirstOrDefault();
      if (referee != null)
      {
        var s = new Schedule()
        {
          RefereeId = referee.Id,
          Referee = referee,
          AvailableAt = DateTime.UtcNow,
        };
        _context.Schedules.Add(s);
        _context.SaveChanges();
      }
    }

    public void AddCompany(string name, int y)
    {
      _context.Companies.Add(new Company()
      {
        Name = name,
        Address = name + " gatan " + y,
        Email = name + y + "@" + name + ".com",
        City = name,
        HasValidSubscription = true,
        SubscriptionActiveFrom = DateTime.UtcNow,
        // Seeddata company subscription-time valid 1 year from creation.
        SubscriptionEndsAt = DateTime.UtcNow.AddDays(365),
        PhoneOne = "019123456",
        PhoneTwo = "019654321",
        Sports = new List<CompanySport>() {
          new CompanySport() { SportType = SportType.Fotboll },
          new CompanySport() { SportType = SportType.Ishockey },
        }
      });
      _context.SaveChanges();
    }

    public void AddCompanyAndScheduleFirstReferee(string name, int y)
    {
      _context.Companies.Add(new Company()
      {
        Name = name,
        Address = name + " gatan " + y,
        Email = name + y + "@" + name + ".com",
        City = name,
        HasValidSubscription = true,
        SubscriptionActiveFrom = DateTime.UtcNow,
        // Seeddata company subscription-time valid 1 year from creation.
        SubscriptionEndsAt = DateTime.UtcNow.AddDays(365),
        PhoneOne = "019123456",
        PhoneTwo = "019654321",
        Sports = new List<CompanySport>() {
          new CompanySport() { SportType = SportType.Fotboll },
          new CompanySport() { SportType = SportType.Ishockey },
        }
      });
      _context.SaveChanges();

      var c = _context.Companies.Where(x => x.Id == y).FirstOrDefault();
      var s = _context.Schedules.Where(x => x.Id == y).FirstOrDefault();

      if (c != null && s != null)
      {
        _context.BookingRequestsByCompany.Add(new BookingRequestByCompany()
        {
          CompanyId = c.Id,
          Message = "Kan du komma på vår match?",
          RefereeType = RefereeType.Hudvuddomare,
          SportType = SportType.Fotboll,
          RequestingCompany = c,
          Schedule = s,
          ScheduleId = s.Id
        });
      }
      else
      {
        System.Console.WriteLine("Company or Schedule not found by ID!");
      }
      _context.SaveChanges();
    }

    public void AddEventForCompany(string name, int y)
    {
      _context.CompanyEvents.Add(new CompanyEvent()
      {
        Name = name,
        Date = DateTime.UtcNow,
        Location = "Ölands jumpasal",
        CompanyId = y,
        RefereeTypesForEvent = new List<RefereeTypesCompanyEvent>() {
          new RefereeTypesCompanyEvent { RefereeType = RefereeType.Hudvuddomare },
          new RefereeTypesCompanyEvent { RefereeType = RefereeType.Linjeman },
          new RefereeTypesCompanyEvent { RefereeType = RefereeType.Linjeman },
        }
      });
      _context.SaveChanges();
    }

    public void AddRefereeForEvent(string message, int y)
    {
      _context.BookingRequestsByReferee.Add(new BookingRequestByReferee()
      {
        CompanyEventId = y,
        RefereeId = y,
        Message = message,
        AppliedAt = DateTime.UtcNow,
      });
    }

    public bool RespondYes(int y)
    {
      var s = _context.Schedules.Where(x => x.Id == y).FirstOrDefault();
      var r = _context.BookingRequestsByCompany.Where(x => x.Id == y).FirstOrDefault();

      if (s != null && r != null)
      {
        r.Accepted = true;
        r.RespondedAt = DateTime.UtcNow;
        _context.SaveChanges();
      }

      return true;
    }

    public bool AddCounty(int y)
    {
      Referee referee = _context.Referees.Where(x => x.Id == y).FirstOrDefault();
      var countys = new List<County>() {
        new County() { RefereeId = y, CountyType = CountyType.Orebro },
        new County() { RefereeId = y, CountyType = CountyType.Varmland },
      };
      if (referee != null)
      {
        referee.Countys = countys;
        _context.SaveChanges();
      }

      return true;
    }
  }
}