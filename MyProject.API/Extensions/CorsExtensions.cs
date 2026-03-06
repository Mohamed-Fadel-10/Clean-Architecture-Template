namespace Api.Extensions
{
    public static class CorsExtensions
    {
        /// <summary>
        /// Configures CORS policies.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The configured service collection.</returns>
        public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin",
                    obuilder =>
                    {
                        obuilder
                            .WithOrigins(configuration["App:CorsOrigins"]?.Split(",", StringSplitOptions.RemoveEmptyEntries) ?? [])
                            .WithExposedHeaders("x-pagination")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            return services;
        }
    }
}
