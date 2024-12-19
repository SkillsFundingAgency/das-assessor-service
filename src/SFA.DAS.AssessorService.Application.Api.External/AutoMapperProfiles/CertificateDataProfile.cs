using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Extenstions;
using SFA.DAS.AssessorService.AutoMapperExtensions;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CertificateDataProfile : Profile
    {
        public CertificateDataProfile()
        {
            // Request going to Int API
            CreateMap<Models.Request.Certificates.CertificateData, Domain.JsonData.CertificateData>()
                .IgnoreUnmappedMembers()
                .ForMember(dest => dest.LearnerFamilyName, opt => opt.MapFrom(source => source.Learner.FamilyName))
                .ForMember(dest => dest.AchievementDate, opt => opt.MapFrom(source => source.LearningDetails.AchievementDate))
                .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.Standard.StandardReference))
                .ForMember(dest => dest.CourseOption, opt => opt.MapFrom(source => source.LearningDetails.CourseOption))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(source => source.LearningDetails.Version))
                .ForMember(dest => dest.OverallGrade, opt => opt.MapFrom(source => source.LearningDetails.OverallGrade))
                .ForMember(dest => dest.ContactName, opt => opt.MapFrom(source => source.PostalContact.ContactName))
                .ForMember(dest => dest.ContactOrganisation, opt => opt.MapFrom(source => source.PostalContact.Organisation))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(source => source.PostalContact.Department))
                .ForMember(dest => dest.ContactAddLine1, opt => opt.MapFrom(source => source.PostalContact.AddressLine1))
                .ForMember(dest => dest.ContactAddLine2, opt => opt.MapFrom(source => source.PostalContact.AddressLine2))
                .ForMember(dest => dest.ContactAddLine3, opt => opt.MapFrom(source => source.PostalContact.AddressLine3))
                .ForMember(dest => dest.ContactAddLine4, opt => opt.MapFrom(source => source.PostalContact.City))
                .ForMember(dest => dest.ContactPostCode, opt => opt.MapFrom(source => source.PostalContact.PostCode))
                .ForMember(dest => dest.CoronationEmblem, opt => opt.MapFrom(source => source.CoronationEmblem));

            // Response from Int API
            CreateMap<Domain.JsonData.CertificateData, Models.Response.Certificates.CertificateData>()
                .IgnoreUnmappedMembers()
                .ForMember(dest => dest.CertificateReference, opt => opt.MapFrom(source => string.Empty))
                .ForPath(dest => dest.Learner.FamilyName, opt => opt.MapFrom(source => source.LearnerFamilyName))
                .ForPath(dest => dest.Learner.GivenNames, opt => opt.MapFrom(source => source.LearnerGivenNames))
                .ForPath(dest => dest.LearningDetails.Version, opt => opt.MapFrom(source => source.Version))
                .ForPath(dest => dest.LearningDetails.AchievementDate, opt => opt.MapFrom(source => source.AchievementDate.DropMilliseconds()))
                .ForPath(dest => dest.LearningDetails.CourseOption, opt => opt.MapFrom(source => source.CourseOption))
                .ForPath(dest => dest.LearningDetails.Version, opt => {
                        opt.Condition(version => !string.IsNullOrEmpty(version.SourceMember));
                        opt.MapFrom(source => source.Version);
                    })
                .ForPath(dest => dest.LearningDetails.LearningStartDate, opt => opt.MapFrom(source => source.LearningStartDate))
                .ForPath(dest => dest.LearningDetails.OverallGrade, opt => opt.MapFrom(source => source.OverallGrade))
                .ForPath(dest => dest.LearningDetails.ProviderName, opt => opt.MapFrom(source => source.ProviderName))
                .ForPath(dest => dest.Standard.StandardReference, opt => opt.MapFrom(source => source.StandardReference))
                .ForPath(dest => dest.Standard.StandardName, opt => opt.MapFrom(source => source.StandardName))
                .ForPath(dest => dest.Standard.Level, opt => opt.MapFrom(source => source.StandardLevel))
                .ForPath(dest => dest.PostalContact.AddressLine1, opt => opt.MapFrom(source => source.ContactAddLine1))
                .ForPath(dest => dest.PostalContact.AddressLine2, opt => opt.MapFrom(source => source.ContactAddLine2))
                .ForPath(dest => dest.PostalContact.AddressLine3, opt => opt.MapFrom(source => source.ContactAddLine3))
                .ForPath(dest => dest.PostalContact.City, opt => opt.MapFrom(source => source.ContactAddLine4))
                .ForPath(dest => dest.PostalContact.ContactName, opt => opt.MapFrom(source => source.ContactName))
                .ForPath(dest => dest.PostalContact.Department, opt => opt.MapFrom(source => source.Department))
                .ForPath(dest => dest.PostalContact.Organisation, opt => opt.MapFrom(source => source.ContactOrganisation))
                .ForPath(dest => dest.PostalContact.PostCode, opt => opt.MapFrom(source => source.ContactPostCode))
                .ForPath(dest => dest.CoronationEmblem, opt => opt.MapFrom(source => source.CoronationEmblem));
        }
    }
}
