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

namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<CreateOrganisationRequest, Organisation>();
            CreateMap<UpdateOrganisationRequest, Organisation>();

            CreateMap<CreateContactRequest, Contact>().ReverseMap();
            CreateMap<Contact, ContactResponse>();
            CreateMap<Learner, SearchResult>()
                .ForMember(dest => dest.Option, source => source.MapFrom(learner => learner.CourseOption))
                .ForMember(dest => dest.UpdatedAt, source => source.MapFrom(learner => learner.LastUpdated));
            CreateMap<Learner, StaffSearchItems>()
                .ForMember(q => q.StandardCode, opts => { opts.MapFrom(i => i.StdCode); });

            CreateMap<CreateBatchLogRequest, BatchLog>();
            CreateMap<BatchData, BatchDataResponse>();
            CreateMap<BatchLog, BatchLogResponse>()
                .ForMember(q => q.BatchData, opts => { opts.MapFrom(q => q.BatchData); });

            CreateMap<Certificate, CertificateResponse>()
                .ForMember(q => q.EndPointAssessorOrganisationId,
                    opts => { opts.MapFrom(q => q.Organisation.EndPointAssessorOrganisationId); })
                .ForMember(q => q.EndPointAssessorOrganisationName,
                    opts => { opts.MapFrom(q => q.Organisation.EndPointAssessorName); })
                .ForMember(q => q.BatchNumber,
                    opts => { opts.MapFrom<BatchNumberResolver>(); });

            CreateMap<string, CertificateDataResponse>()
                .ConvertUsing<JsonMappingConverter<CertificateDataResponse>>();

            CreateMap<Certificate, CertificateSummaryResponse>();

            CreateMap<CreateEpaOrganisationRequest, EpaOrganisationResponse>();
            CreateMap<UpdateEpaOrganisationRequest, EpaOrganisationResponse>();
            CreateMap<CreateEpaOrganisationStandardRequest, EpaoStandardResponse>();
            CreateMap<UpdateEpaOrganisationStandardRequest, EpaoStandardResponse>();
            CreateMap<AddressResponse, GetAddressResponse>().ReverseMap();

        }
    }
}
