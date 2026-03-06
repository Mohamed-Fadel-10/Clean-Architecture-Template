using Microsoft.EntityFrameworkCore;
using Shared.Pagination;

namespace Infrastructure.Persistence.Extensions
{
    public static class QueryablePaginationExtensions
    {
        public static async Task<PagedList<T>> ToPagedList<T>(
            this IQueryable<T> source,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);

            var items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedList<T>(items, count, pageIndex, pageSize);
        }
    }
}