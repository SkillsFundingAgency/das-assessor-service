using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers
{
    public class UpdateOptionalParamatersWithDefaultValues : IOperationFilter
    {
        private readonly JsonSerializer _jsonSerializer;

        public UpdateOptionalParamatersWithDefaultValues(IOptions<MvcNewtonsoftJsonOptions> mvcJsonOptions)
        {
            _jsonSerializer = JsonSerializer.CreateDefault(mvcJsonOptions.Value.SerializerSettings);
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) return;
            var parameterValuePairs = context.ApiDescription.ParameterDescriptions
                .Where(parameter => GetDefaultValueAttribute(parameter) != null || GetParameterInfo(parameter).HasDefaultValue)
                .ToDictionary(parameter => parameter.Name, GetDefaultValue);

            foreach (var parameter in operation.Parameters)
            {
                if (parameterValuePairs.TryGetValue(parameter.Name, out var defaultValue))
                {
                    //parameter.Required = false; <-- swagger hates this for some reason!
                    if (defaultValue != null)
                    {
                        var jValue = (JValue)JValue.FromObject(defaultValue, _jsonSerializer);

                        // .NetCore 3.1 upgrade @ToDo: test this change
                        //parameter.Extensions.Add("default", jValue.Value);
                        parameter.Extensions.Add("default", new OpenApiString(jValue.Value.ToString()));
                    }
                    else
                    {
                        parameter.Extensions.Add("default", null);
                    }
                }
            }
        }

        private DefaultValueAttribute GetDefaultValueAttribute(ApiParameterDescription parameter)
        {
            return (parameter.ModelMetadata as DefaultModelMetadata)?
                .Attributes.PropertyAttributes?
                .OfType<DefaultValueAttribute>()
                .FirstOrDefault();
        }

        public ParameterInfo GetParameterInfo(ApiParameterDescription parameter)
        {
            return ((ControllerParameterDescriptor)parameter.ParameterDescriptor).ParameterInfo;
        }

        private object GetDefaultValue(ApiParameterDescription parameter)
        {
            var parameterInfo = GetParameterInfo(parameter);
            if (parameterInfo.HasDefaultValue)
            {
                return parameterInfo.DefaultValue;
            }
            else
            {
                return GetDefaultValueAttribute(parameter)?.Value;
            }
        }
    }
}
