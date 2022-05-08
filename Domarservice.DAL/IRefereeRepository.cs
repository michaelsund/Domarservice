using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domarservice.DAL
{
    public interface IRefereeRepository
    {
        Referee GetRefeereById(int id);
    }
}
