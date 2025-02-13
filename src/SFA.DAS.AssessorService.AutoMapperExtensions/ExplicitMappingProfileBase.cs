using AutoMapper;

namespace SFA.DAS.AssessorService.AutoMapperExtensions
{
    public class ExplicitMappingProfileBase :Profile
    {
        protected ExplicitMappingProfileBase()
        {
            ShouldMapProperty = propertyInfo => false;
            ShouldMapField = fieldInfo => false;
        }
    }
}
