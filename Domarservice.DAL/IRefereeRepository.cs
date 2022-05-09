using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domarservice.Models;

namespace Domarservice.DAL
{
    public interface IRefereeRepository
    {
        RefereeDto GetRefeereById(int id);
        List<Referee> GetAllReferees();
    }
}
