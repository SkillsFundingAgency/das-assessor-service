using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CertificateDataProfile : Profile
    {
        public CertificateDataProfile()
        {
            CreateMap<Models.Certificates.CertificateData, Domain.JsonData.CertificateData>()
                .ForMember(x => x.LearnerGivenNames, opt => opt.MapFrom(source => source.Learner.GivenNames))
                .ForMember(x => x.LearnerFamilyName, opt => opt.MapFrom(source => source.Learner.FamilyName))
                .ForMember(x => x.FullName, opt => opt.MapFrom(source => $"{source.Learner.GivenNames} {source.Learner.FamilyName}"))
                .ForMember(x => x.StandardName, opt => opt.MapFrom(source => source.LearningDetails.StandardName))
                .ForMember(x => x.StandardLevel, opt => opt.MapFrom(source => source.LearningDetails.StandardLevel))
                .ForMember(x => x.StandardPublicationDate, opt => opt.MapFrom(source => source.LearningDetails.StandardPublicationDate))
                .ForMember(x => x.LearningStartDate, opt => opt.MapFrom(source => source.LearningDetails.LearningStartDate))
                .ForMember(x => x.AchievementDate, opt => opt.MapFrom(source => source.LearningDetails.AchievementDate))
                .ForMember(x => x.CourseOption, opt => opt.MapFrom(source => source.LearningDetails.CourseOption))
                .ForMember(x => x.OverallGrade, opt => opt.MapFrom(source => source.LearningDetails.OverallGrade))
                .ForMember(x => x.ContactName, opt => opt.MapFrom(source => source.PostalContact.ContactName))
                .ForMember(x => x.ContactOrganisation, opt => opt.MapFrom(source => source.PostalContact.Organisation))
                .ForMember(x => x.Department, opt => opt.MapFrom(source => source.PostalContact.Department))
                .ForMember(x => x.ContactAddLine1, opt => opt.MapFrom(source => source.PostalContact.AddressLine1))
                .ForMember(x => x.ContactAddLine2, opt => opt.MapFrom(source => source.PostalContact.AddressLine2))
                .ForMember(x => x.ContactAddLine3, opt => opt.MapFrom(source => source.PostalContact.AddressLine3))
                .ForMember(x => x.ContactAddLine4, opt => opt.MapFrom(source => source.PostalContact.City))
                .ForMember(x => x.ContactPostCode, opt => opt.MapFrom(source => source.PostalContact.PostCode))
                .ForMember(x => x.ProviderName, opt => opt.MapFrom(source => source.LearningDetails.ProviderName))
                .ForMember(x => x.Registration, opt => opt.MapFrom(source => string.Empty))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Domain.JsonData.CertificateData, Models.Certificates.CertificateData>()
                .ForMember(x => x.CertificateReference, opt => opt.MapFrom(source => string.Empty))
                .ForPath(x => x.Learner.FamilyName, opt => opt.MapFrom(source => source.LearnerFamilyName))
                .ForPath(x => x.Learner.GivenNames, opt => opt.MapFrom(source => source.LearnerGivenNames))
                .ForPath(x => x.LearningDetails.AchievementDate, opt => opt.MapFrom(source => source.AchievementDate))
                .ForPath(x => x.LearningDetails.CourseOption, opt => opt.MapFrom(source => source.CourseOption))
                .ForPath(x => x.LearningDetails.LearningStartDate, opt => opt.MapFrom(source => source.LearningStartDate))
                .ForPath(x => x.LearningDetails.OverallGrade, opt => opt.MapFrom(source => source.OverallGrade))
                .ForPath(x => x.LearningDetails.ProviderName, opt => opt.MapFrom(source => source.ProviderName))
                .ForPath(x => x.LearningDetails.StandardLevel, opt => opt.MapFrom(source => source.StandardLevel))
                .ForPath(x => x.LearningDetails.StandardName, opt => opt.MapFrom(source => source.StandardName))
                .ForPath(x => x.LearningDetails.StandardPublicationDate, opt => opt.MapFrom(source => source.StandardPublicationDate))
                .ForPath(x => x.PostalContact.AddressLine1, opt => opt.MapFrom(source => source.ContactAddLine1))
                .ForPath(x => x.PostalContact.AddressLine2, opt => opt.MapFrom(source => source.ContactAddLine2))
                .ForPath(x => x.PostalContact.AddressLine3, opt => opt.MapFrom(source => source.ContactAddLine3))
                .ForPath(x => x.PostalContact.City, opt => opt.MapFrom(source => source.ContactAddLine4))
                .ForPath(x => x.PostalContact.ContactName, opt => opt.MapFrom(source => source.ContactName))
                .ForPath(x => x.PostalContact.Department, opt => opt.MapFrom(source => source.Department))
                .ForPath(x => x.PostalContact.Organisation, opt => opt.MapFrom(source => source.ContactOrganisation))
                .ForPath(x => x.PostalContact.PostCode, opt => opt.MapFrom(source => source.ContactPostCode))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
