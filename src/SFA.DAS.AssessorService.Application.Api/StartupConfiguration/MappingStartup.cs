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
using SFA.DAS.AssessorService.AutoMapperExtensions;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class MappingStartup
    {
        public static void AddMappings(this IServiceCollection services, ILogger logger)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateOrganisationRequest, Organisation>()
                    .IgnoreUnmappedMembers();

                cfg.CreateMap<UpdateOrganisationRequest, Organisation>()
                    .IgnoreUnmappedMembers();

                cfg.CreateMap<CreateContactRequest, Contact>()
                .IgnoreUnmappedMembers()
                    .ForMember(dest => dest.GivenNames, opt => opt.MapFrom(src => src.GivenName))
                    .ForMember(dest => dest.GovUkIdentifier, opt => opt.MapFrom(src => src.GovIdentifier))
                    .ReverseMap();

                cfg.CreateMap<Learner, StaffSearchItems>()
                    .IgnoreUnmappedMembers()
                    .ForMember(q => q.StandardCode, opts => { opts.MapFrom(i => i.StdCode); });

                cfg.CreateMap<CreateBatchLogRequest, BatchLog>()
                    .IgnoreUnmappedMembers();
                cfg.CreateMap<BatchData, BatchDataResponse>()
                    .IgnoreUnmappedMembers();
                cfg.CreateMap<BatchLog, BatchLogResponse>()
                    .IgnoreUnmappedMembers()
                    .ForMember(q => q.BatchData, opts => { opts.MapFrom(q => q.BatchData); });

                cfg.CreateMap<Certificate, CertificateResponse>()
                    .IgnoreUnmappedMembers()
                    .ForMember(q => q.EndPointAssessorOrganisationId,
                        opts => { opts.MapFrom(q => q.Organisation.EndPointAssessorOrganisationId); })
                    .ForMember(q => q.EndPointAssessorOrganisationName,
                        opts => { opts.MapFrom(q => q.Organisation.EndPointAssessorName); })
                    .ForMember(q => q.BatchNumber,
                        opts => { opts.MapFrom<BatchNumberResolver>(); });

                cfg.CreateMap<string, CertificateDataResponse>()
                    .ConvertUsing<JsonMappingConverter<CertificateDataResponse>>();

                cfg.CreateMap<Certificate, CertificateSummaryResponse>()
                    .IgnoreUnmappedMembers();

                cfg.AddProfile<EpaOrganisationProfile>();
                cfg.AddProfile<OppFinderProfile>();
                cfg.AddProfile<ApplicationResponseProfile>();

                cfg.CreateMap<AddressResponse, GetAddressResponse>().ReverseMap();
                cfg.CreateMap<Contact, ContactResponse>();
                cfg.CreateMap<Organisation, OrganisationResponse>()
                    .IgnoreUnmappedMembers();

                cfg.CreateMap<ApplicationListItem, ApplicationSummaryItem>()
                    .ForMember(dest => dest.Versions, opt => opt.Ignore())
                    .ForMember(dest => dest.WithdrawalType, opt => opt.Ignore());
            });


            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

#if DEBUF

            mapper.PrintMappings<Organisation, OrganisationResponse>();
            mapper.PrintMappings<EpaOrganisation, UpdateEpaOrganisationRequest>();
            mapper.PrintMappings<ApplicationListItem, ApplicationSummaryItem>();

            string referencingAssemblyPath = Assembly.GetExecutingAssembly().Location;
            MapCallAnalyzer.FindAndPrintMapCalls(referencingAssemblyPath, mapper.ConfigurationProvider);

#endif
        }

    }
}