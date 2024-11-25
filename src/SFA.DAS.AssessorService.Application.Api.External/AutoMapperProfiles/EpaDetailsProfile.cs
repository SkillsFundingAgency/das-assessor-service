using AutoMapper;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class EpaDetailsProfile : Profile
    {
        public EpaDetailsProfile()
        {
            // Request going to Int API
            CreateMap<Models.Request.Epa.EpaDetails, Domain.JsonData.EpaDetails>()
                .IgnoreUnmappedMembers()
                .BeforeMap((source, dest) => dest.LatestEpaDate = null)
                .BeforeMap((source, dest) => dest.LatestEpaOutcome = null)
                .ForMember(dest => dest.Epas, opt => opt.MapFrom(source => source.Epas));

            // Response from Int API
            CreateMap<Domain.JsonData.EpaDetails, Models.Response.Epa.EpaDetails>()
                .IgnoreUnmappedMembers()
                .ForMember(dest => dest.EpaReference, opt => opt.MapFrom(source => source.EpaReference))
                .ForMember(dest => dest.LatestEpaDate, opt => opt.MapFrom(source => source.LatestEpaDate))
                .ForMember(dest => dest.LatestEpaOutcome, opt => opt.MapFrom(source => source.LatestEpaOutcome))
                .ForMember(dest => dest.Epas, opt => opt.MapFrom(source => source.Epas));

            // Response from Int API - if using CertificateData
            CreateMap<Domain.JsonData.CertificateData, Models.Response.Epa.EpaDetails>()
                .IgnoreUnmappedMembers()
                .ForMember(dest => dest.EpaReference, opt => opt.ResolveUsing(source => source.EpaDetails?.EpaReference))
                .ForMember(dest => dest.LatestEpaDate, opt => opt.ResolveUsing(source => source.EpaDetails?.LatestEpaDate))
                .ForMember(dest => dest.LatestEpaOutcome, opt => opt.ResolveUsing(source => source.EpaDetails?.LatestEpaOutcome))
                .ForMember(dest => dest.Epas, opt => opt.ResolveUsing(source => source.EpaDetails?.Epas));
        }
    }
}
