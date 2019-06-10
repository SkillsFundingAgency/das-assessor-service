using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class CertificateDataProfile : Profile
    {
        // TODO: This needs a proper check.
        // Especially as the requests are now missing certain things and we need to check the CertData in the back end has everything!
        public CertificateDataProfile()
        {
            // Request going to Int API
            CreateMap<Models.Request.Certificates.CertificateData, Domain.JsonData.CertificateData>()
                //.ForMember(x => x.LearnerGivenNames, opt => opt.MapFrom(source => source.Learner.GivenNames))
                .ForMember(x => x.LearnerFamilyName, opt => opt.MapFrom(source => source.Learner.FamilyName))
                //.ForMember(x => x.FullName, opt => opt.MapFrom(source => $"{source.Learner.GivenNames} {source.Learner.FamilyName}"))
                //.ForMember(x => x.StandardName, opt => opt.MapFrom(source => source.Standard.StandardName))
                //.ForMember(x => x.StandardLevel, opt => opt.MapFrom(source => source.Standard.Level))
                //.ForMember(x => x.LearningStartDate, opt => opt.MapFrom(source => source.LearningDetails.LearningStartDate))
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
                //.ForMember(x => x.ProviderName, opt => opt.MapFrom(source => source.LearningDetails.ProviderName))
                .ForAllOtherMembers(x => x.Ignore());

            // Response from Int API
            CreateMap<Domain.JsonData.CertificateData, Models.Response.Certificates.CertificateData>()
                .ForMember(x => x.CertificateReference, opt => opt.MapFrom(source => string.Empty))
                .ForPath(x => x.Learner.FamilyName, opt => opt.MapFrom(source => source.LearnerFamilyName))
                .ForPath(x => x.Learner.GivenNames, opt => opt.MapFrom(source => source.LearnerGivenNames))
                .ForPath(x => x.LearningDetails.AchievementDate, opt => opt.MapFrom(source => source.AchievementDate))
                .ForPath(x => x.LearningDetails.CourseOption, opt => opt.MapFrom(source => source.CourseOption))
                .ForPath(x => x.LearningDetails.LearningStartDate, opt => opt.MapFrom(source => source.LearningStartDate))
                .ForPath(x => x.LearningDetails.OverallGrade, opt => opt.MapFrom(source => source.OverallGrade))
                .ForPath(x => x.LearningDetails.ProviderName, opt => opt.MapFrom(source => source.ProviderName))
                .ForPath(x => x.Standard.StandardName, opt => opt.MapFrom(source => source.StandardName))
                .ForPath(x => x.Standard.Level, opt => opt.MapFrom(source => source.StandardLevel))
                .ForPath(x => x.PostalContact.AddressLine1, opt => opt.MapFrom(source => source.ContactAddLine1))
                .ForPath(x => x.PostalContact.AddressLine2, opt => opt.MapFrom(source => source.ContactAddLine2))
                .ForPath(x => x.PostalContact.AddressLine3, opt => opt.MapFrom(source => source.ContactAddLine3))
                .ForPath(x => x.PostalContact.City, opt => opt.MapFrom(source => source.ContactAddLine4))
                .ForPath(x => x.PostalContact.ContactName, opt => opt.MapFrom(source => source.ContactName))
                .ForPath(x => x.PostalContact.Department, opt => opt.MapFrom(source => source.Department))
                .ForPath(x => x.PostalContact.Organisation, opt => opt.MapFrom(source => source.ContactOrganisation))
                .ForPath(x => x.PostalContact.PostCode, opt => opt.MapFrom(source => source.ContactPostCode))
                .ForAllOtherMembers(x => x.Ignore());

            // These ones are required for replaying back stuff to the 'front end'
            CreateMap<Models.Response.Certificates.CertificateData, Models.Request.Certificates.CertificateData>()
                .ForMember(x => x.CertificateReference, opt => opt.MapFrom(source => source.CertificateReference))
                .ForPath(x => x.Learner.Uln, opt => opt.MapFrom(source => source.Learner.Uln))
                .ForPath(x => x.Learner.FamilyName, opt => opt.MapFrom(source => source.Learner.FamilyName))
                .ForPath(x => x.LearningDetails.AchievementDate, opt => opt.MapFrom(source => source.LearningDetails.AchievementDate))
                .ForPath(x => x.LearningDetails.CourseOption, opt => opt.MapFrom(source => source.LearningDetails.CourseOption))
                .ForPath(x => x.LearningDetails.OverallGrade, opt => opt.MapFrom(source => source.LearningDetails.OverallGrade))
                .ForPath(x => x.Standard.StandardCode, opt => opt.MapFrom(source => source.Standard.StandardCode))
                .ForPath(x => x.Standard.StandardReference, opt => opt.MapFrom(source => source.Standard.StandardReference))
                .ForPath(x => x.PostalContact.AddressLine1, opt => opt.MapFrom(source => source.PostalContact.AddressLine1))
                .ForPath(x => x.PostalContact.AddressLine2, opt => opt.MapFrom(source => source.PostalContact.AddressLine2))
                .ForPath(x => x.PostalContact.AddressLine3, opt => opt.MapFrom(source => source.PostalContact.AddressLine3))
                .ForPath(x => x.PostalContact.City, opt => opt.MapFrom(source => source.PostalContact.City))
                .ForPath(x => x.PostalContact.PostCode, opt => opt.MapFrom(source => source.PostalContact.PostCode))
                .ForPath(x => x.PostalContact.ContactName, opt => opt.MapFrom(source => source.PostalContact.ContactName))
                .ForPath(x => x.PostalContact.Department, opt => opt.MapFrom(source => source.PostalContact.Department))
                .ForPath(x => x.PostalContact.Organisation, opt => opt.MapFrom(source => source.PostalContact.Organisation))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<Models.Request.Certificates.CertificateData, Models.Response.Certificates.CertificateData>()
                .ForMember(x => x.CertificateReference, opt => opt.MapFrom(source => source.CertificateReference))
                .ForPath(x => x.Learner.Uln, opt => opt.MapFrom(source => source.Learner.Uln))
                .ForPath(x => x.Learner.FamilyName, opt => opt.MapFrom(source => source.Learner.FamilyName))
                .ForPath(x => x.LearningDetails.AchievementDate, opt => opt.MapFrom(source => source.LearningDetails.AchievementDate))
                .ForPath(x => x.LearningDetails.CourseOption, opt => opt.MapFrom(source => source.LearningDetails.CourseOption))
                .ForPath(x => x.LearningDetails.OverallGrade, opt => opt.MapFrom(source => source.LearningDetails.OverallGrade))
                .ForPath(x => x.Standard.StandardCode, opt => opt.MapFrom(source => source.Standard.StandardCode))
                .ForPath(x => x.Standard.StandardReference, opt => opt.MapFrom(source => source.Standard.StandardReference))
                .ForPath(x => x.PostalContact.AddressLine1, opt => opt.MapFrom(source => source.PostalContact.AddressLine1))
                .ForPath(x => x.PostalContact.AddressLine2, opt => opt.MapFrom(source => source.PostalContact.AddressLine2))
                .ForPath(x => x.PostalContact.AddressLine3, opt => opt.MapFrom(source => source.PostalContact.AddressLine3))
                .ForPath(x => x.PostalContact.City, opt => opt.MapFrom(source => source.PostalContact.City))
                .ForPath(x => x.PostalContact.PostCode, opt => opt.MapFrom(source => source.PostalContact.PostCode))
                .ForPath(x => x.PostalContact.ContactName, opt => opt.MapFrom(source => source.PostalContact.ContactName))
                .ForPath(x => x.PostalContact.Department, opt => opt.MapFrom(source => source.PostalContact.Department))
                .ForPath(x => x.PostalContact.Organisation, opt => opt.MapFrom(source => source.PostalContact.Organisation))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
