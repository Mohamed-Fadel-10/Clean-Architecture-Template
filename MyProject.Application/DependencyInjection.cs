using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Application.Behaviors;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();


            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
            });
            // FluentValidation
            services.AddValidatorsFromAssembly(assembly);
            // Pipeline Behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }


    }
}
