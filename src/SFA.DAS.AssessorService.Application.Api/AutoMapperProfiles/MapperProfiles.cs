using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Mapping.CustomResolvers;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;
using Contact = SFA.DAS.AssessorService.Domain.Entities.Contact;
using Learner = SFA.DAS.AssessorService.Domain.Entities.Learner;

namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public class MapperProfiles: Profile
    {
        public MapperProfiles()
        {
            CreateMap<Contact, ContactResponse>();
            CreateMap<AddressResponse, GetAddressResponse>();
        }
    }
}
