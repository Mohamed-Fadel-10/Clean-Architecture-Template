using Application.Abstractions.Authentication;
using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Persistence.Interceptors
{
    public sealed class UpdateAuditableEntitiesInterceptor(IUserContext userContext) : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
        {
            var applicationUser = userContext.CurrentUserId;

            var dbContext = eventData.Context;

            if (dbContext is null)
            {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            // track auditable changes Added / Modified / Deleted  states 
            var entries = dbContext.ChangeTracker.Entries<IAuditable>();
            foreach (var entityEntry in entries)
            {
                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        entityEntry.Property(o => o.CreatedAt).CurrentValue = DateTime.UtcNow;
                        entityEntry.Property(o => o.CreatedBy).CurrentValue = applicationUser;
                        break;

                    case EntityState.Modified:
                        entityEntry.Property(o => o.CreatedAt).IsModified = false;
                        entityEntry.Property(o => o.CreatedBy).IsModified = false;
                        entityEntry.Property(o => o.UpdatedAt).CurrentValue = DateTime.UtcNow;
                        entityEntry.Property(o => o.UpdatedBy).CurrentValue = applicationUser;
                        break;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
