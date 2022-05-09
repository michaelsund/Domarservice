using AutoMapper;
using System.Linq;
using Domarservice.DAL;
using Domarservice.Models;

namespace Domarservice
{
  public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
      CreateMap<Referee, RefereeDto>();
      CreateMap<Sport, SportDto>();
    }
  }
}