using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params string[] includes) where T : BaseEntity
    {
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }
}