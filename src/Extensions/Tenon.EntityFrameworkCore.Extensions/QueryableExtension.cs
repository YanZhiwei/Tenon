using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tenon.EntityFrameworkCore.Extensions.Models;

namespace Tenon.EntityFrameworkCore.Extensions;

public static class QueryableExtension
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        int pageIndex,
        int pageSize,
        CancellationToken token = default)
    {
        var totalCount = await source.CountAsync(token);
        if (totalCount > 0)
        {
            var items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
            return new PagedList<T>(items, pageIndex,pageSize, totalCount);
        }

        return new PagedList<T>(Enumerable.Empty<T>(), 0, 0, 0);
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }
}