using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.Models;

namespace Domarservice.DAL
{
  public interface IRefereeRepository
  {
    Task<RefereeDto> GetRefeereById(int id);
    Task<SimpleRefereeDto> GetSimpleRefeereById(int id);
    // List<Referee> GetAllReferees();
    Task<Referee> CreateReferee();
    Task<bool> DeleteRefereeById(int? id);
  }
}
