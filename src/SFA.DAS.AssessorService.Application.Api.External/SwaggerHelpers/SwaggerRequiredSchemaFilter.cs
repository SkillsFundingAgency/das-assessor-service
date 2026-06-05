using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers
{
    public class SwaggerRequiredSchemaFilter : ISchemaFilter
    {
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties is null) return;

            foreach (var schemaProperty in schema.Properties)
            {
                var property = context.Type.GetProperty(schemaProperty.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    var attributes = property.GetCustomAttributes(true);

                    if (attributes != null && attributes.Any(attr => attr is Attributes.SwaggerRequiredAttribute))
                    {                        
                        schema.Required.Add(schemaProperty.Key);
                    }
                }
            }
        }     
    }
}
