using System;

public class ValidFromAndTo
{
  public bool Check(DateTime From, DateTime To)
  {
    if ((From.Year == To.Year) && (From.Month == To.Month) && (From.Day == To.Day))
    {
      if (From.Hour < To.Hour)
      {
        return true;
      }
      else if (From.Hour == To.Hour)
      {
        if (From.Minute < To.Minute)
        {
          return true;
        }
      }
    }
    return false;
  }
}