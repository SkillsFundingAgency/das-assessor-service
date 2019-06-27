using AutoMapper;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class EpaDetailsProfile : Profile
    {
        public EpaDetailsProfile()
        {
            // Request going to Int API
            CreateMap<Models.Request.Epa.EpaDetails, Domain.JsonData.EpaDetails>()
                .BeforeMap((source, dest) => dest.LatestEpaDate = null)
                .BeforeMap((source, dest) => dest.LatestEpaOutcome = null)
                .ForMember(dest => dest.Epas, opt => opt.MapFrom(source => Mapper.Map<IEnumerable<Models.Request.Epa.EpaRecord>, IEnumerable<Domain.JsonData.EpaRecord>>(source.Epas)))
                .ForAllOtherMembers(dest => dest.Ignore());

            // Response from Int API
            CreateMap<Domain.JsonData.EpaDetails, Models.Response.Epa.EpaDetails>()
                .ForMember(dest => dest.LatestEpaDate, opt => opt.MapFrom(source => source.LatestEpaDate))
                .ForMember(dest => dest.LatestEpaOutcome, opt => opt.MapFrom(source => source.LatestEpaOutcome))
                .ForMember(dest => dest.Epas, opt => opt.MapFrom(source => Mapper.Map<IEnumerable<Domain.JsonData.EpaRecord>, IEnumerable<Models.Response.Epa.EpaDetails>>(source.Epas)))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
