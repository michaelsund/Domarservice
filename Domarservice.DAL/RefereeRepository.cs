using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace Domarservice.DAL
{
  public class RefereeRepository : IRefereeRepository
  {
    private readonly DomarserviceContext _context = null;

    public RefereeRepository(DomarserviceContext context)
    {
      _context = context;
    }

    public Referee GetRefeereById(int id)
    {
      var referee = new Referee();
      try
      {
        referee = _context.Referees.Where(x => x.Id == id).FirstOrDefault();
      }
      catch (System.Exception)
      {
        throw;
      }
      return referee;
    }
  }
}
