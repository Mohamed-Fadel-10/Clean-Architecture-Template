using Domain.ResultPattern;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

namespace API.Extensions
{
    public static class JwtExtensions
    {
        public static IServiceCollection ConfigureJwtOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    ValidIssuer = configuration["JwtSettings:Issuer"] ?? "NoorTabark",
                    ValidAudience = configuration["JwtSettings:Audience"] ?? "NoorTabarkUsers",
                    IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"] ??
                throw new InvalidOperationException("JWT Key is not configured")))
                };

                o.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("=== TOKEN VALIDATION SUCCESS ===");
                        Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("=== AUTHENTICATION FAILED ===");
                        Console.WriteLine($"Error: {context.Exception.Message}");

                        if (context.Exception is SecurityTokenInvalidIssuerException)
                        {
                            Console.WriteLine($"Expected Issuer: {o.TokenValidationParameters.ValidIssuer}");
                        }
                        else if (context.Exception is SecurityTokenInvalidAudienceException)
                        {
                            Console.WriteLine($"Expected Audience: {o.TokenValidationParameters.ValidAudience}");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();
            return services;
        }
    }
}