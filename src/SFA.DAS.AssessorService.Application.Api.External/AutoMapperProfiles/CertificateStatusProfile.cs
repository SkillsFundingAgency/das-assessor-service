using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CertificateStatusProfile : Profile
    {
        public CertificateStatusProfile()
        {
            CreateMap<Domain.Entities.Certificate, Models.Certificates.Status>()
            .ForMember(x => x.CurrentStatus, opt => opt.MapFrom(source => source.Status))
            .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Domain.Entities.Certificate, Models.Certificates.Created>()
            .ForMember(x => x.CreatedAt, opt => opt.MapFrom(source => source.CreatedAt))
            .ForMember(x => x.CreatedBy, opt => opt.MapFrom(source => source.CreatedBy))
            .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Domain.Entities.Certificate, Models.Certificates.Submitted>()
            .ForMember(x => x.SubmittedAt, opt => opt.MapFrom(source => source.UpdatedAt))
            .ForMember(x => x.SubmittedBy, opt => opt.MapFrom(source => source.UpdatedBy))
            .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Domain.Entities.Certificate, Models.Certificates.Printed>()
            .ForMember(x => x.PrintedAt, opt => opt.MapFrom(source => source.ToBePrinted))
            .ForMember(x => x.PrintedBatch, opt => opt.MapFrom(source => source.BatchNumber))
            .ForAllOtherMembers(x => x.Ignore());            
        }
    }
}
