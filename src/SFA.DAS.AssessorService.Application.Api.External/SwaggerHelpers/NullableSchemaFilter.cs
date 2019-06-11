using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers
{
    public class NullableSchemaFilter : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            if (model.Properties is null) return;

            foreach (var schemaProperty in model.Properties)
            {
                var property = context.SystemType.GetProperty(schemaProperty.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null && property.PropertyType.IsConstructedGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    schemaProperty.Value.Default = null;
                    //schemaProperty.Value.Extensions.Add("nullable", true); <-- APIM hasn't moved onto OpenAPI 3.0 so cannot use this at the moment
                    schemaProperty.Value.Extensions.Add("x-nullable", true);
                    schemaProperty.Value.Example = null;
                }
            }
        }
    }
}
