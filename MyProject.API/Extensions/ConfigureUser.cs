using Api.ContextServices;
using Application.Abstractions.Authentication;
using Domain.Common.Interfaces;

namespace API.Extensions
{

    public static class ConfigureUser
    {
        public static IServiceCollection ConfigureUserContext(this IServiceCollection services)
        {
            services.AddScoped<IUserContext, UserContext>();

            return services;
        }
    }

}
