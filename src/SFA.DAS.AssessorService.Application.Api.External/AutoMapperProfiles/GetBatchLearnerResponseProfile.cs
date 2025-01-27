using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners;
using SFA.DAS.AssessorService.Application.Api.External.Extenstions;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Learners;
using SFA.DAS.AssessorService.AutoMapperExtensions;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class GetBatchLearnerResponseProfile : Profile
    {
        public GetBatchLearnerResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearnerResponse>()
            .IgnoreUnmappedMembers()
            .ForMember(dest => dest.Learner, opt => opt.MapFrom(source => source))
            .ForMember(dest => dest.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors));
        }
    }

    public class GetLearnerProfile : Profile
    {
        public GetLearnerProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearner>()
                .IgnoreUnmappedMembers()
                .ForMember(dest => dest.Certificate, opt => opt.MapFrom(source => source.Certificate))
                .ForMember(dest => dest.EpaDetails, opt => opt.MapFrom(source => source.EpaDetails))
                .AfterMap<MapStatusAction>()
                .AfterMap<MapLearnerDataAction>()
                .AfterMap<HideCertificateAction>()
                .AfterMap<CollapseNullsAction>();
        }

        public class MapLearnerDataAction : IMappingAction<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearner>
        {
            public void Process(AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse source, GetLearner destination, ResolutionContext context)
            {
                // Always use the learner first if it is there!
                if (source.Learner != null)
                {
                    destination.LearnerData = new LearnerData
                    {
                        Standard = source.Learner.Standard is null ? null : new Standard { StandardCode = source.Learner.Standard.LarsCode, StandardReference = source.Learner.Standard.IFateReferenceNumber, StandardName = source.Learner.Standard.Title, Level = source.Learner.Standard.Level },
                        Learner = new Learner 
                        { 
                            Uln = source.Learner.Uln, 
                            GivenNames = source.Learner.GivenNames, 
                            FamilyName = source.Learner.FamilyName
                        },
                        LearningDetails = new Models.Response.Learners.LearningDetails 
                        { 
                            LearnerReferenceNumber = source.Learner.LearnerReferenceNumber, 
                            ProviderUkPrn = source.Learner.UkPrn, 
                            ProviderName = source.Learner.OrganisationName, 
                            LearningStartDate = source.Learner.LearnerStartDate.DropMilliseconds(), 
                            PlannedEndDate = source.Learner.PlannedEndDate.DropMilliseconds(),
                            Version = GetVersionFromGetBatchLearnerResponse(source, destination),
                            CourseOption = GetCourseOptionFromGetBatchLearnerResponse(source, destination)
                        }
                    };
                }
                else if (destination.Certificate?.CertificateData != null)
                {
                    var certData = destination.Certificate.CertificateData;
                    destination.LearnerData = new LearnerData
                    {
                        Standard = certData.Standard,
                        Learner = certData.Learner,
                        LearningDetails = new Models.Response.Learners.LearningDetails 
                        { 
                            ProviderUkPrn = certData.LearningDetails.ProviderUkPrn, 
                            ProviderName = certData.LearningDetails.ProviderName, 
                            LearningStartDate = certData.LearningDetails.LearningStartDate.DropMilliseconds()
                        }
                    };
                }
            }

            private string GetVersionFromGetBatchLearnerResponse(GetBatchLearnerResponse source, GetLearner destination)
            {
                if (!string.IsNullOrEmpty(source.Learner.Version))
                {
                    return source.Learner.Version;
                }
                else if (!string.IsNullOrWhiteSpace(destination.Certificate?.CertificateData?.LearningDetails?.Version))
                {
                    return destination.Certificate.CertificateData.LearningDetails.Version;
                }

                return null;
            }

            private string GetCourseOptionFromGetBatchLearnerResponse(GetBatchLearnerResponse source, GetLearner destination)
            {
                if (!string.IsNullOrEmpty(source.Learner.CourseOption))
                {
                    return source.Learner.CourseOption;
                }
                else if (!string.IsNullOrWhiteSpace(destination.Certificate?.CertificateData?.LearningDetails?.CourseOption))
                {
                    return destination.Certificate.CertificateData.LearningDetails.CourseOption;
                }

                return null;
            }
        }

        public class MapStatusAction : IMappingAction<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearner>
        {
            public void Process(AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse source, GetLearner destination, ResolutionContext context)
            {
                if (source.Learner?.CompletionStatus != null)
                {
                    destination.Status = new Models.Response.Learners.Status
                    {
                        CompletionStatus = source.Learner.CompletionStatus
                    };
                }
                else if (destination.Certificate?.Status?.CurrentStatus != null)
                {
                    // NOTE: This block of code allows us to determine the completionStatus based on Certificate Status
                    int? completionStatus;
                    switch (destination.Certificate.Status.CurrentStatus)
                    {
                        case CertificateStatus.Submitted:
                        case CertificateStatus.Printed:
                        case CertificateStatus.Reprint:
                            completionStatus = 2; // completed
                            break;
                        case CertificateStatus.Deleted:
                            completionStatus = null; // unknown
                            break;
                        default:
                            completionStatus = 1; // active
                            break;
                    }

                    if (completionStatus != null)
                    {
                        destination.Status = new Models.Response.Learners.Status
                        {
                            CompletionStatus = completionStatus
                        };
                    }
                }
            }
        }

        public class HideCertificateAction : IMappingAction<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearner>
        {
            public void Process(AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse source, GetLearner destination, ResolutionContext context)
            {
                if (destination.Certificate is null) return;

                if (destination.Certificate.Status?.CurrentStatus == CertificateStatus.Deleted)
                {
                    destination.Certificate = null;
                }
            }
        }

        public class CollapseNullsAction : IMappingAction<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearner>
        {
            public void Process(AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse source, GetLearner destination, ResolutionContext context)
            {
                if (destination.EpaDetails != null && string.IsNullOrEmpty(destination.EpaDetails.LatestEpaOutcome))
                {
                    destination.EpaDetails = null;
                }

                if (destination.Status != null && !destination.Status.CompletionStatus.HasValue)
                {
                    destination.Status = null;
                }
            }
        }
    }
}
