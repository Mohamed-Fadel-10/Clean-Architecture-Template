using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Persistence.Data;

namespace API.Extensions
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("sqlConnection"));
            });

            return services;
        }
    }
}
