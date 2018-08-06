using AutoMapper;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CertificateProfile : Profile
    {
        public CertificateProfile()
        {
            CreateMap<Domain.Entities.Certificate, Models.Certificates.Certificate>()
                .ForMember(x => x.CertificateData, opt => opt.MapFrom(source => Mapper.Map<Domain.JsonData.CertificateData, Models.Certificates.CertificateData>(JsonConvert.DeserializeObject<Domain.JsonData.CertificateData>(source.CertificateData))))
                .ForPath(x => x.CertificateData.CertificateReference, opt => opt.MapFrom(source => source.CertificateReference))
                .ForPath(x => x.CertificateData.Learner.Uln, opt => opt.MapFrom(source => source.Uln))
                .ForPath(x => x.CertificateData.LearningDetails.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
                .ForMember(x => x.Status, opt => opt.MapFrom(source => source.Status))
                .ForMember(x => x.CreatedAt, opt => opt.MapFrom(source => source.CreatedAt))
                .ForMember(x => x.CreatedBy, opt => opt.MapFrom(source => source.CreatedBy))
                .ForMember(x => x.UpdatedAt, opt => opt.MapFrom(source => source.UpdatedAt))
                .ForMember(x => x.UpdatedBy, opt => opt.MapFrom(source => source.UpdatedBy))
                .ForMember(x => x.DeletedAt, opt => opt.MapFrom(source => source.DeletedAt))
                .ForMember(x => x.DeletedBy, opt => opt.MapFrom(source => source.DeletedBy))
                .ForMember(x => x.PrintedAt, opt => opt.MapFrom(source => source.ToBePrinted))
                .ForMember(x => x.BatchNumber, opt => opt.MapFrom(source => source.BatchNumber))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
