using Application.Abstractions.Authentication;
using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Persistence.Contexts.Interceptors
{
    public sealed class SoftDeleteInterceptor(IUserContext userContext) : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
        {
            if (!userContext.SoftDeleteEnabled)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }
            var applicationUser = userContext.CurrentUserId;

            var dbContext = eventData.Context;

            if (dbContext is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }
            var deletedEntries = dbContext.ChangeTracker.Entries<IDeletable>();
            foreach (var deletedEntry in deletedEntries)
            {
                if (deletedEntry.State == EntityState.Deleted)
                {
                    deletedEntry.Property(o => o.DeletedAt).CurrentValue = DateTime.UtcNow;
                    deletedEntry.Property(o => o.DeletedBy).CurrentValue = applicationUser;
                    deletedEntry.Property(o => o.IsDeleted).CurrentValue = true;
                    deletedEntry.State = EntityState.Modified;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
