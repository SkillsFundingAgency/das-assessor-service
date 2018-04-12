using AutoMapper;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Application.Mapping.CustomResolvers
{
    public class JsonMappingConverter<T> : ITypeConverter<string, T>
    {
        public T Convert(string source, T destination, ResolutionContext context)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }
    }
}
