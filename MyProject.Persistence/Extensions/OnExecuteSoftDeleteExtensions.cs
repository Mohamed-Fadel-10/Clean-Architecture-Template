using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common.Interfaces;

namespace Persistence.Extensions
{
    public static class SoftDeleteExtensions
    {
        /// <summary>
        /// Executes a soft delete operation by updating IsDeleted, DeletedAt, and DeletedBy.
        /// </summary>
        public static Task<int> ExecuteSoftDeleteAsync<T>( this IQueryable<T> query, string? deletedBy = null, CancellationToken cancellationToken = default)where T : class, IDeletable
        {
            return query.ExecuteUpdateAsync(
                s => s
                    .SetProperty(x => x.IsDeleted, true)
                    .SetProperty(x => x.DeletedAt, DateTime.UtcNow)
                    .SetProperty(x => x.DeletedBy, (Guid?)(deletedBy == null ? null : Guid.Parse(deletedBy))),
                cancellationToken
            );
        }
    }
}
