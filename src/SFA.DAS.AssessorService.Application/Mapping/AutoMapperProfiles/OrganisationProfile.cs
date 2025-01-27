using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;


namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class CreateOrganisationRequestProfile : Profile
    {
        public CreateOrganisationRequestProfile()
        {
            CreateMap<CreateOrganisationRequest, Organisation>();
        }
    }

    public class UpdateOrganisationRequestProfile : Profile
    {
        public UpdateOrganisationRequestProfile()
        {
            CreateMap<UpdateOrganisationRequest, Organisation>();
        }
    }

    public class OrganisationResponseProfile : Profile
    {
        public OrganisationResponseProfile()
        {
            CreateMap<Organisation, OrganisationResponse>();
        }
    }
}
