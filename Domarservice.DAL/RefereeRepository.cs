using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using Domarservice.Models;

namespace Domarservice.DAL
{
  public class RefereeRepository : IRefereeRepository
  {
    private readonly DomarserviceContext _context = null;

    public RefereeRepository(DomarserviceContext context)
    {
      _context = context;
    }

    public RefereeDto GetRefeereById(int id)
    {
      var referee = _context.Referees
      .Where(x => x.Id == id)
      .Select(x => new RefereeDto()
        {
          Id = x.Id,
          Surname = x.Surname,
          Lastname = x.Lastname
        }
      ).SingleOrDefault();
      return referee;
    }
  }
}
