using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using System;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CertificateProfile : Profile
    {
        public CertificateProfile()
        {
            CreateMap<Domain.Entities.Certificate, Certificate>()
                .ForMember(dest => dest.CertificateData, opt => opt.MapFrom(source => Mapper.Map<Domain.JsonData.CertificateData, CertificateData>(JsonConvert.DeserializeObject<Domain.JsonData.CertificateData>(source.CertificateData))))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => Mapper.Map<Domain.Entities.Certificate, Status>(source)))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(source => Mapper.Map<Domain.Entities.Certificate, Created>(source)))
                .ForMember(dest => dest.Submitted, opt => opt.MapFrom(source => Mapper.Map<Domain.Entities.Certificate, Submitted>(source)))
                .ForMember(dest => dest.Printed, opt => opt.MapFrom(source => Mapper.Map<Domain.Entities.Certificate, Printed>(source)))
                .ForPath(dest => dest.CertificateData.CertificateReference, opt => opt.MapFrom(source => source.CertificateReference))
                .ForPath(dest => dest.CertificateData.Learner.Uln, opt => opt.MapFrom(source => source.Uln))
                .ForPath(dest => dest.CertificateData.Standard.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
                .AfterMap<MapProviderUkPrnAction>()
                .AfterMap<CollapseNullsAction>()
                .ForAllOtherMembers(dest => dest.Ignore());
        }

        public class MapProviderUkPrnAction : IMappingAction<Domain.Entities.Certificate, Certificate>
        {
            public void Process(Domain.Entities.Certificate source, Certificate destination)
            {
                if (destination.CertificateData.LearningDetails != null)
                {
                    destination.CertificateData.LearningDetails.ProviderUkPrn = source.ProviderUkPrn;
                }
            }
        }

        public class CollapseNullsAction : IMappingAction<Domain.Entities.Certificate, Certificate>
        {
            public void Process(Domain.Entities.Certificate source, Certificate destination)
            {
                if (destination.Created.CreatedBy is null)
                {
                    destination.Created = null;
                }

                if (destination.Submitted.SubmittedBy is null)
                {
                    destination.Submitted = null;
                }

                if (destination.Printed.PrintedAt is null)
                {
                    destination.Printed = null;
                }

                if (destination.CertificateData != null)
                {
                    if (destination.CertificateData.LearningDetails?.LearningStartDate == DateTime.MinValue)
                    {
                        destination.CertificateData.LearningDetails = null;
                    }

                    if (destination.CertificateData.PostalContact?.PostCode is null)
                    {
                        destination.CertificateData.PostalContact = null;
                    }
                }
            }
        }
    }
}
