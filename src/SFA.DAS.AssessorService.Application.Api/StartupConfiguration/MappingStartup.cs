using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Mapping.CustomResolvers;
using SFA.DAS.AssessorService.Domain.Entities;
using Contact = SFA.DAS.AssessorService.Domain.Entities.Contact;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Organisation, OrganisationResponse>();
                cfg.CreateMap<CreateOrganisationRequest, Organisation>();
                cfg.CreateMap<UpdateOrganisationRequest, Organisation>();
                cfg.CreateMap<CreateContactRequest, Contact>().ReverseMap();                
                cfg.CreateMap<Contact, ContactResponse>();
                cfg.CreateMap<Ilr, SearchResult>();
                cfg.CreateMap<Certificate, CertificateResponse>()
                    .ForMember(q => q.EndPointAssessorOrganisationId,
                        opts => { opts.MapFrom(q => q.Organisation.EndPointAssessorOrganisationId); })
                    .ForMember(q => q.BatchNumber,
                        opts => { opts.ResolveUsing<BatchNumberResolver>(); });

                cfg.CreateMap<string, CertificateDataResponse>()
                    .ConvertUsing<JsonMappingConverter<CertificateDataResponse>>();
            });
        }
    }
}