using System;
using System.Collections.Generic;

namespace Domarservice.Helpers
{
  public class DateHelper
  {
    public static IEnumerable<DateTime> AllDaysInMonth(int year, int month)
    {
      int days = DateTime.DaysInMonth(year, month);
      for (int day = 1; day <= days; day++)
      {
        yield return new DateTime(year, month, day);
      }
    }
  }
}