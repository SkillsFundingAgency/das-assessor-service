using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CertificateStatusProfile : Profile
    {
        public CertificateStatusProfile()
        {
            CreateMap<Domain.Entities.Certificate, Status>()
            .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(source => source.Status))
            .ForAllOtherMembers(dest => dest.Ignore());

            CreateMap<Domain.Entities.Certificate, Created>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(source => source.CreatedAt))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(source => source.CreatedBy))
            .ForAllOtherMembers(dest => dest.Ignore());

            CreateMap<Domain.Entities.Certificate, Submitted>()
            .ForMember(dest => dest.SubmittedAt, opt => opt.MapFrom(source => source.UpdatedAt))
            .ForMember(dest => dest.SubmittedBy, opt => opt.MapFrom(source => source.UpdatedBy))
            .ForAllOtherMembers(dest => dest.Ignore());

            CreateMap<Domain.Entities.Certificate, Printed>()
            .ForMember(dest => dest.PrintedAt, opt => opt.MapFrom(source => source.ToBePrinted))
            .ForMember(dest => dest.PrintedBatch, opt => opt.MapFrom(source => source.BatchNumber))
            .ForAllOtherMembers(dest => dest.Ignore());            
        }
    }
}
