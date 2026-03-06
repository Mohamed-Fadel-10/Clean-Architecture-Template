using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Common.Interfaces;
using Domain.Entities.RefreshToken.RefreshToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyProject.Infrastructure.Identity;
using Persistence.Contexts;
using Persistence.Contexts.Interceptors;
using Persistence.Interceptors;

public sealed class Context : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>,IApplicationDbContext
{
    private readonly IUserContext _userContext;

    public Context(DbContextOptions<Context> options, IUserContext userContext)
        : base(options)
    {
        _userContext = userContext;
    }

    public DbSet<User> Users { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);
        builder.ApplyQueryFilterToAssignableEntities<IDeletable>(x => !x.IsDeleted);
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(
            new UpdateAuditableEntitiesInterceptor(_userContext),
            new SoftDeleteInterceptor(_userContext)
        );
        base.OnConfiguring(optionsBuilder);
    }
}
