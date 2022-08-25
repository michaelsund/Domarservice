using System;
using System.Linq;
using System.Linq.Expressions;

namespace Domarservice.Helpers
{
  public static class LinqExtensions
  {
    public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
    {
      return condition ? source.Where(predicate) : source;
    }
  }
}
