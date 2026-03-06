using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerConfigurations
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllParametersInCamelCase();

                options.OperationFilter<FileUploadOperationFilter>();
                options.CustomSchemaIds(type => type.FullName);

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "1",
                    Description = "Through this API you can access User details",
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token in this format - Bearer token to access this API",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });

                options.OperationFilter<AddAcceptLanguageHeaderParameter>();

            });

            return services;
        }

        public class FileUploadOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var consumesAttribute = context.MethodInfo
                    .GetCustomAttributes(true)
                    .OfType<ConsumesAttribute>()
                    .FirstOrDefault();

                if (consumesAttribute?.ContentTypes.Contains("multipart/form-data") != true)
                    return;

                var fileParams = context.MethodInfo.GetParameters()
                    .Where(p => p.ParameterType == typeof(IFormFile) ||
                               p.ParameterType == typeof(List<IFormFile>) ||
                               p.ParameterType == typeof(IEnumerable<IFormFile>))
                    .ToList();

                if (fileParams.Count == 0)
                    return;

                operation.Parameters?.Clear();

                var properties = new Dictionary<string, OpenApiSchema>();

                foreach (var param in context.MethodInfo.GetParameters())
                {
                    if (param.ParameterType == typeof(IFormFile))
                    {
                        properties[param.Name ?? "file"] = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        };
                    }
                    else if (param.ParameterType == typeof(List<IFormFile>) ||
                             param.ParameterType == typeof(IEnumerable<IFormFile>))
                    {
                        properties[param.Name ?? "files"] = new OpenApiSchema
                        {
                            Type = "array",
                            Items = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            }
                        };
                    }
                    else if (param.ParameterType == typeof(string))
                    {
                        properties[param.Name ?? "unknown"] = new OpenApiSchema
                        {
                            Type = "string",
                            Nullable = true
                        };
                    }
                    else if (param.ParameterType == typeof(int) || param.ParameterType == typeof(int?))
                    {
                        properties[param.Name ?? "unknown"] = new OpenApiSchema
                        {
                            Type = "integer",
                            Nullable = param.ParameterType == typeof(int?)
                        };
                    }
                }

                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = properties
                            }
                        }
                    }
                };
            }
        }
    }
}