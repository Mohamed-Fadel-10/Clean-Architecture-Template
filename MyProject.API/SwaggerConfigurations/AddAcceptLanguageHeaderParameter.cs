using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerConfigurations
{

    /// <summary>
    /// 
    /// </summary>
    public  class AddAcceptLanguageHeaderParameter : IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= [];

            // Check if Accept-Language header already exists
            var hasAcceptLanguageHeader = operation.Parameters.Any(p => p.Name.Equals("Accept-Language", StringComparison.InvariantCultureIgnoreCase));

            if (!hasAcceptLanguageHeader)
            {
                // Add Accept-Language header parameter
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Accept-Language",
                    In = ParameterLocation.Header,
                    Required = false, // Set to true if it's mandatory
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Default = new OpenApiString("ar")
                    }
                });
            }
        }
    }
}
