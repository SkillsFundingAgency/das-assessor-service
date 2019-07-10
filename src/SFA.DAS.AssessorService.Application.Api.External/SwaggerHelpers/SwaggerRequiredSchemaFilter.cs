using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers
{
    public class SwaggerRequiredSchemaFilter : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            if (model.Properties is null) return;

            foreach (var schemaProperty in model.Properties)
            {
                var property = context.SystemType.GetProperty(schemaProperty.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    var attributes = property?.GetCustomAttributes(true);

                    if (attributes != null && attributes.Any(attr => attr is Attributes.SwaggerRequiredAttribute))
                    {
                        if (model.Required is null) model.Required = new List<string>();
                        model.Required.Add(schemaProperty.Key);
                    }
                }
            }
        }
    }
}
