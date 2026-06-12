using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Mapping.CustomResolvers;
using SFA.DAS.AssessorService.Domain.Entities;


namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public class CertificateResponseProfile : Profile
    {
        public CertificateResponseProfile()
        {
            CreateMap<Certificate, CertificateResponse>()
                .ForMember(q => q.EndPointAssessorOrganisationId,
                    opts => { opts.MapFrom(q => q.Organisation.EndPointAssessorOrganisationId); })
                .ForMember(q => q.EndPointAssessorOrganisationName,
                    opts => { opts.MapFrom(q => q.Organisation.EndPointAssessorName); })
                .ForMember(q => q.BatchNumber,
                    opts => { opts.MapFrom<BatchNumberResolver>(); });

        }
    }

    public class CertificateDataResponseProfile : Profile
    {
        public CertificateDataResponseProfile()
        {
            CreateMap<string, CertificateDataResponse>()
                .ConvertUsing<JsonMappingConverter<CertificateDataResponse>>();
        }
    }

    public class CertificateSummaryResponseProfile : Profile
    {
        public CertificateSummaryResponseProfile()
        {
            CreateMap<Certificate, CertificateSummaryResponse>();
        }
    }
}
