using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.External.Extenstions;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CertificateProfile : Profile
    {
        public CertificateProfile()
        {
            CreateMap<Domain.Entities.Certificate, Certificate>()
                .IgnoreUnmappedMembers()
                .ForMember(dest => dest.CertificateData, opt => opt.MapFrom(source => JsonConvert.DeserializeObject<Domain.JsonData.CertificateData>(source.CertificateData ?? "")))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(source => source))
                .ForMember(dest => dest.Submitted, opt => opt.MapFrom(source => source))
                .ForMember(dest => dest.Printed, opt => opt.MapFrom(source => source))
                .ForMember(dest => dest.Delivered, opt => opt.MapFrom(source => source.CertificateLogs
                    .Where(log => log.Status == CertificateStatus.Delivered || log.Status == CertificateStatus.NotDelivered)
                    .OrderByDescending(log => log.EventTime)
                    .FirstOrDefault()))
                .ForPath(dest => dest.CertificateData.CertificateReference, opt => opt.MapFrom(source => source.CertificateReference))
                .ForPath(dest => dest.CertificateData.Learner.Uln, opt => opt.MapFrom(source => source.Uln))
                .ForPath(dest => dest.CertificateData.Standard.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
                .AfterMap<MapProviderUkPrnAction>()
                .AfterMap<CollapseNullsAction>();

            CreateMap<Domain.Entities.CertificateLog, Delivered>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(source => source.Status))
                .ForMember(dest => dest.DeliveryDate, opt => opt.MapFrom(source => source.EventTime.DropMilliseconds()));
        }

        public class MapProviderUkPrnAction : IMappingAction<Domain.Entities.Certificate, Certificate>
        {
            public void Process(Domain.Entities.Certificate source, Certificate destination, ResolutionContext context)
            {
                if (destination.CertificateData.LearningDetails != null)
                {
                    destination.CertificateData.LearningDetails.ProviderUkPrn = source.ProviderUkPrn;
                }
            }
        }

        public class CollapseNullsAction : IMappingAction<Domain.Entities.Certificate, Certificate>
        {
            public void Process(Domain.Entities.Certificate source, Certificate destination, ResolutionContext context)
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
