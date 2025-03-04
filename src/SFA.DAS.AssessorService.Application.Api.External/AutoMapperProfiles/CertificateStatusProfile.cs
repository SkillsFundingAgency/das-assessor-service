using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Extenstions;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.AutoMapperExtensions;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CertificateStatusProfile : ExplicitMappingProfileBase
    {
        public CertificateStatusProfile()
        {
            CreateMap<Domain.Entities.Certificate, Status>()
            .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(source => source.Status));

            CreateMap<Domain.Entities.Certificate, Created>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(source => source.CreatedAt.DropMilliseconds()))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(source => source.CreatedBy));

            CreateMap<Domain.Entities.Certificate, Submitted>()
            .ForMember(dest => dest.SubmittedAt, opt => opt.MapFrom(source => source.UpdatedAt.DropMilliseconds()))
            .ForMember(dest => dest.SubmittedBy, opt => opt.MapFrom(source => source.UpdatedBy));

            CreateMap<Domain.Entities.Certificate, Printed>()
            .ForMember(dest => dest.PrintedAt, opt => opt.MapFrom(source => source.ToBePrinted.DropMilliseconds()))
            .ForMember(dest => dest.PrintedBatch, opt => opt.MapFrom(source => source.BatchNumber));
        }
    }
}
