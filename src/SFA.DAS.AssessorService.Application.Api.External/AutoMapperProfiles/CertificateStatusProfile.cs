using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CertificateStatusProfile : Profile
    {
        public CertificateStatusProfile()
        {
            CreateMap<Domain.Entities.Certificate, Models.Certificates.CertificateStatus>()
            .ForMember(x => x.CurrentStatus, opt => opt.MapFrom(source => source.Status))
            .ForMember(x => x.CreatedAt, opt => opt.MapFrom(source => source.CreatedAt))
            .ForMember(x => x.CreatedBy, opt => opt.MapFrom(source => source.CreatedBy))
            .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(source => source.UpdatedAt))
            .ForMember(x => x.UpdatedBy, opt => opt.MapFrom(source => source.UpdatedBy))
            .ForMember(x => x.DeletedAt, opt => opt.MapFrom(source => source.DeletedAt))
            .ForMember(x => x.DeletedBy, opt => opt.MapFrom(source => source.DeletedBy))
            .ForMember(x => x.PrintedAt, opt => opt.MapFrom(source => source.ToBePrinted))
            .ForMember(x => x.PrintedBatch, opt => opt.MapFrom(source => source.BatchNumber))
            .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
