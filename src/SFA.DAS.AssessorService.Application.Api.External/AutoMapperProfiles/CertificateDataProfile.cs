using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Extenstions;
using SFA.DAS.AssessorService.AutoMapperExtensions;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CertificateDataProfile : ExplicitMappingProfileBase
    {
        public CertificateDataProfile()
        {
            // Request going to Int API
            CreateMap<Models.Request.Certificates.CertificateData, Domain.JsonData.CertificateData>()
                .ForMember(dest => dest.LearnerFamilyName, opt => opt.MapFrom(source => source.Learner.FamilyName))
                .ForMember(dest => dest.AchievementDate, opt => opt.MapFrom(source => source.LearningDetails.AchievementDate))
                .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.Standard.StandardReference))
                .ForMember(dest => dest.CourseOption, opt => opt.MapFrom(source => source.LearningDetails.CourseOption))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(source => source.LearningDetails.Version))
                .ForMember(dest => dest.OverallGrade, opt => opt.MapFrom(source => source.LearningDetails.OverallGrade))
                .ForMember(dest => dest.ContactName, opt => opt.Ignore())
                .ForMember(dest => dest.ContactOrganisation, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.ContactAddLine1, opt => opt.Ignore())
                .ForMember(dest => dest.ContactAddLine2, opt => opt.Ignore())
                .ForMember(dest => dest.ContactAddLine3, opt => opt.Ignore())
                .ForMember(dest => dest.ContactAddLine4, opt => opt.Ignore())
                .ForMember(dest => dest.ContactPostCode, opt => opt.Ignore())
                .ForMember(dest => dest.SendTo, opt => opt.Ignore())
                .ForMember(dest => dest.CoronationEmblem, opt => opt.MapFrom(source => source.CoronationEmblem));

            // Response from Int API
            CreateMap<Domain.JsonData.CertificateData, Models.Response.Certificates.CertificateData>()
                .ForMember(dest => dest.CertificateReference, opt => opt.MapFrom(source => string.Empty))
                .ForPath(dest => dest.Learner.FamilyName, opt => opt.MapFrom(source => source.LearnerFamilyName))
                .ForPath(dest => dest.Learner.GivenNames, opt => opt.MapFrom(source => source.LearnerGivenNames))
                .ForPath(dest => dest.LearningDetails.AchievementDate, opt => opt.MapFrom(source => source.AchievementDate.DropMilliseconds()))
                .ForPath(dest => dest.LearningDetails.CourseOption, opt => opt.MapFrom(source => source.CourseOption))
                .ForPath(dest => dest.LearningDetails.Version, opt =>
                {
                    opt.Condition(version => !string.IsNullOrEmpty(version.SourceMember));
                    opt.MapFrom(source => source.Version);
                })
                .ForPath(dest => dest.LearningDetails.LearningStartDate, opt => opt.MapFrom(source => source.LearningStartDate))
                .ForPath(dest => dest.LearningDetails.OverallGrade, opt => opt.MapFrom(source => source.OverallGrade))
                .ForPath(dest => dest.LearningDetails.ProviderName, opt => opt.MapFrom(source => source.ProviderName))
                .ForPath(dest => dest.Standard.StandardReference, opt => opt.MapFrom(source => source.StandardReference))
                .ForPath(dest => dest.Standard.StandardName, opt => opt.MapFrom(source => source.StandardName))
                .ForPath(dest => dest.Standard.Level, opt => opt.MapFrom(source => source.StandardLevel))
                .ForMember(dest => dest.PostalContact, opt => opt.MapFrom(source => (Models.Response.Certificates.PostalContact)null))
                .ForPath(dest => dest.CoronationEmblem, opt => opt.MapFrom(source => source.CoronationEmblem));
        }
    }
}
