using Api.ContextServices;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.ExternalServices.FileStorageService;
using Persistence.ExternalServices.JWTService;



namespace Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {

           services.AddScoped<IFileStorageService, FileStorageService>();

            services.AddHttpContextAccessor();
            services.AddScoped<IApplicationDbContext>(sp =>
                    sp.GetRequiredService<Context>());
            
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IJWTService, JWTService>();
            return services;
        }
    }
}
