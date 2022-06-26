﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.Models;

namespace Domarservice.DAL
{
  public interface IRefereeRepository
  {
    Task<RefereeDto> GetRefeereById(int id);
    // List<Referee> GetAllReferees();
    Task<bool> DeleteRefereeById(int? id);
  }
}
