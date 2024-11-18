using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles;
using SFA.DAS.AssessorService.Application.Mapping.CustomResolvers;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;
using Contact = SFA.DAS.AssessorService.Domain.Entities.Contact;
using Learner = SFA.DAS.AssessorService.Domain.Entities.Learner;
using SFA.DAS.AssessorService.ApplyTypes;
using CreateOrganisationRequest = SFA.DAS.AssessorService.Api.Types.Models.CreateOrganisationRequest;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateOrganisationRequest, Organisation>()
                    .MapMatchingMembersAndIgnoreOthers();

                cfg.CreateMap<UpdateOrganisationRequest, Organisation>()
                    .MapMatchingMembersAndIgnoreOthers();
                   
                cfg.CreateMap<CreateContactRequest, Contact>()
                .MapMatchingMembersAndIgnoreOthers()
                    .ForMember(dest => dest.GivenNames, opt => opt.MapFrom(src => src.GivenName))
                    .ForMember(dest => dest.GovUkIdentifier, opt => opt.MapFrom(src => src.GovIdentifier))
                    .ReverseMap();

                cfg.CreateMap<Learner, StaffSearchItems>()
                    .MapMatchingMembersAndIgnoreOthers()
                    .ForMember(q => q.StandardCode, opts => { opts.MapFrom(i => i.StdCode); });

                cfg.CreateMap<CreateBatchLogRequest, BatchLog>()
                    .MapMatchingMembersAndIgnoreOthers();
                cfg.CreateMap<BatchData, BatchDataResponse>()
                    .MapMatchingMembersAndIgnoreOthers();
                cfg.CreateMap<BatchLog, BatchLogResponse>()
                    .MapMatchingMembersAndIgnoreOthers()
                    .ForMember(q => q.BatchData, opts => { opts.MapFrom(q => q.BatchData); });

                cfg.CreateMap<Certificate, CertificateResponse>()
                    .MapMatchingMembersAndIgnoreOthers()
                    .ForMember(q => q.EndPointAssessorOrganisationId,
                        opts => { opts.MapFrom(q => q.Organisation.EndPointAssessorOrganisationId); })
                    .ForMember(q => q.EndPointAssessorOrganisationName,
                        opts => { opts.MapFrom(q => q.Organisation.EndPointAssessorName); })
                    .ForMember(q => q.BatchNumber,
                        opts => { opts.MapFrom<BatchNumberResolver>(); });

                cfg.CreateMap<string, CertificateDataResponse>()
                    .ConvertUsing<JsonMappingConverter<CertificateDataResponse>>();

                cfg.CreateMap<Certificate, CertificateSummaryResponse>()
                    .MapMatchingMembersAndIgnoreOthers();

                cfg.AddProfile<EpaOrganisationProfile>();
                cfg.AddProfile<OppFinderProfile>();
                cfg.AddProfile<ApplicationResponseProfile>();

                

                cfg.CreateMap<AddressResponse, GetAddressResponse>().ReverseMap();
                cfg.CreateMap<Contact, ContactResponse>();
                cfg.CreateMap<Organisation, OrganisationResponse>()
                    .MapMatchingMembersAndIgnoreOthers();

                cfg.CreateMap<ApplicationListItem, ApplicationSummaryItem>()
                    .ForMember(dest => dest.Versions, opt => opt.Ignore())
                    .ForMember(dest => dest.WithdrawalType, opt => opt.Ignore());
            });

            services.AddSingleton(config.CreateMapper());

        }

    }
}