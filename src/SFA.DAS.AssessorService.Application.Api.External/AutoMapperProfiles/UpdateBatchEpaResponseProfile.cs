using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.AutoMapperExtensions;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class UpdateBatchEpaResponseProfile : ExplicitMappingProfileBase
    {
        public UpdateBatchEpaResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ExternalApi.Epas.BatchEpaResponse, UpdateEpaResponse>()
            .ForMember(dest => dest.RequestId, opt => opt.MapFrom(source => source.RequestId))
            .ForMember(dest => dest.EpaReference, opt => opt.ResolveUsing(source => source.EpaDetails?.EpaReference))
            .ForMember(dest => dest.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors));
        }
    }
}