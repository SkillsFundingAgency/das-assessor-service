using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CreateBatchEpaResponseProfile : Profile
    {
        public CreateBatchEpaResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ExternalApi.Epas.BatchEpaResponse, CreateEpaResponse>()
            .ForMember(dest => dest.RequestId, opt => opt.MapFrom(source => source.RequestId))
            .ForMember(dest => dest.EpaReference, opt => opt.ResolveUsing(source => source.EpaDetails?.EpaReference))
            .ForMember(dest => dest.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors))
            .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}