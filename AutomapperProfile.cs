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
      CreateMap<Sport, SportDto>();
      CreateMap<Schedule, ScheduleDto>();
      CreateMap<Company, CompanyDto>();
      CreateMap<Referee, RefereeDto>();
      CreateMap<Referee, SimpleRefereeDto>();
      CreateMap<BookingRequest, BookingRequestDto>();
        // .ForMember(t => t.Sports, options => options.MapFrom(source => source.Sports))
        // .ForMember(t => t.Schedules, options => options.MapFrom(source => source.Schedules))
        // .ForMember(t => t.Countys, options => options.MapFrom(source => source.Countys));
    }
  }
}