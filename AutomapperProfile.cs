using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using Domarservice.DAL;
using Domarservice.Models;

namespace Domarservice
{
  public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
      CreateMap<County, CountyDto>();
      CreateMap<RefereeSport, RefereeSportDto>();
      CreateMap<CompanySport, CompanySportDto>();
      CreateMap<Schedule, ScheduleDto>();
      CreateMap<Schedule, SimpleScheduleDto>();
      CreateMap<Company, CompanyDto>();
      CreateMap<Company, SimpleCompanyDto>();
      CreateMap<Company, CompanyAndUsersDto>();
      CreateMap<CompanyEvent, CompanyEventDto>();
      CreateMap<CompanyEvent, ExtendedCompanyEventDto>();
      CreateMap<Referee, RefereeDto>();
      CreateMap<Referee, SimpleRefereeDto>();
      CreateMap<BookingRequestByReferee, BookingRequestByRefereeDto>();
      CreateMap<BookingRequestByCompany, BookingRequestByCompanyDto>();
      CreateMap<RefereeTypesCompanyEvent, RefereeTypesCompanyEventDto>();
      CreateMap<ApplicationUser, SimpleUserDto>();
        // .ForMember(t => t.Sports, options => options.MapFrom(source => source.Sports))
        // .ForMember(t => t.Schedules, options => options.MapFrom(source => source.Schedules))
        // .ForMember(t => t.Countys, options => options.MapFrom(source => source.Countys));
    }
  }
}