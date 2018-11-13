using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using System;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class SearchResultToCertificateProfile : Profile
    {
        public SearchResultToCertificateProfile()
        {
            CreateMap<SearchResult, Models.Certificates.Certificate>()
            .ForPath(x => x.Status.CurrentStatus, opt => opt.MapFrom(source => source.CertificateStatus))
            .ForPath(x => x.Status.CreatedAt, opt => opt.MapFrom(source => source.CreatedAt))
            .ForPath(x => x.Status.CreatedBy, opt => opt.MapFrom(source => source.CreatedBy))
            .ForPath(x => x.Status.UpdatedAt, opt => opt.MapFrom(source => source.UpdatedAt))
            .ForPath(x => x.Status.UpdatedBy, opt => opt.MapFrom(source => source.UpdatedBy))
            .ForPath(x => x.CertificateData.CertificateReference, opt => opt.MapFrom(source => source.CertificateReference))
            .ForPath(x => x.CertificateData.Learner.Uln, opt => opt.MapFrom(source => source.Uln))
            .ForPath(x => x.CertificateData.Learner.GivenNames, opt => opt.MapFrom(source => source.GivenNames))
            .ForPath(x => x.CertificateData.Learner.FamilyName, opt => opt.MapFrom(source => source.FamilyName))
            .ForPath(x => x.CertificateData.Standard.StandardCode, opt => opt.MapFrom(source => source.StdCode))
            .ForPath(x => x.CertificateData.Standard.Name, opt => opt.MapFrom(source => source.Standard))
            .ForPath(x => x.CertificateData.Standard.Level, opt => opt.MapFrom(source => source.Level))
            .ForPath(x => x.CertificateData.LearningDetails.CourseOption, opt => opt.MapFrom(source => source.Option))
            .ForPath(x => x.CertificateData.LearningDetails.LearningStartDate, opt => opt.MapFrom(source => source.LearnStartDate ?? DateTime.MinValue))
            .ForPath(x => x.CertificateData.LearningDetails.OverallGrade, opt => opt.MapFrom(source => source.OverallGrade))
            .ForPath(x => x.CertificateData.LearningDetails.AchievementDate, opt => opt.MapFrom(source => source.AchDate))
            .ForPath(x => x.CertificateData.LearningDetails.ProviderUkPrn, opt => opt.MapFrom(source => source.UkPrn))

            .AfterMap<CollapseNullsAction>()
            .ForAllOtherMembers(x => x.Ignore());
        }

        public class CollapseNullsAction : IMappingAction<SearchResult, Models.Certificates.Certificate>
        {
            public void Process(SearchResult source, Models.Certificates.Certificate destination)
            {
                if (destination.Status?.CreatedBy is null)
                {
                    destination.Status = null;
                }

                if (destination.CertificateData.LearningDetails?.LearningStartDate == DateTime.MinValue)
                {
                    destination.CertificateData.LearningDetails = null;
                }
            }
        }
    }
}
