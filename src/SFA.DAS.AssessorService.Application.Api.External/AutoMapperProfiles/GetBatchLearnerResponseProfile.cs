using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Epa;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Learners;
using SFA.DAS.AssessorService.Domain.Consts;
using System;

namespace SFA.DAS.AssessorService.Application.Api.External.AutoMapperProfiles
{
    public class GetBatchLearnerResponseProfile : Profile
    {
        public GetBatchLearnerResponseProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearnerResponse>()
            .ForMember(dest => dest.Learner, opt => opt.MapFrom(source => Mapper.Map<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearner>(source)))
            .ForMember(dest => dest.ValidationErrors, opt => opt.MapFrom(source => source.ValidationErrors))
            .ForAllOtherMembers(dest => dest.Ignore());
        }
    }

    public class GetLearnerProfile : Profile
    {
        public GetLearnerProfile()
        {
            CreateMap<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearner>()
                .ForMember(dest => dest.Certificate, opt => opt.MapFrom(source => Mapper.Map<Domain.Entities.Certificate, Certificate>(source.Certificate)))
                .ForMember(dest => dest.EpaDetails, opt => opt.ResolveUsing(source => Mapper.Map<Domain.JsonData.CertificateData, EpaDetails>(JsonConvert.DeserializeObject<Domain.JsonData.CertificateData>(source.Certificate?.CertificateData ?? ""))))
                .AfterMap<MapStatusAction>()
                .AfterMap<MapLearnerDataAction>()
                .AfterMap<HideCertificateAction>()
                .AfterMap<CollapseNullsAction>()
                .ForAllOtherMembers(dest => dest.Ignore());
        }

        public class MapLearnerDataAction : IMappingAction<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearner>
        {
            public void Process(AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse source, GetLearner destination)
            {
                // Always use the learner first if it is there!
                if (source.Learner != null)
                {
                    destination.LearnerData = new LearnerData
                    {
                        Standard = source.Learner.Standard is null ? null : new Standard { StandardCode = source.Learner.Standard.StandardId, StandardReference = source.Learner.Standard.ReferenceNumber, StandardName = source.Learner.Standard.Title, Level = source.Learner.Standard.StandardData?.Level ?? 0 },
                        Learner = new Learner { Uln = source.Learner.Uln, GivenNames = source.Learner.GivenNames, FamilyName = source.Learner.FamilyName },
                        LearningDetails = new Models.Response.Learners.LearningDetails { LearnerReferenceNumber = source.Learner.LearnerReferenceNumber, ProviderUkPrn = source.Learner.UkPrn, ProviderName = source.Learner.OrganisationName, LearningStartDate = source.Learner.LearnerStartDate, PlannedEndDate = source.Learner.PlannedEndDate }
                    };
                }
                else if (destination.Certificate?.CertificateData != null)
                {
                    var certData = destination.Certificate.CertificateData;
                    destination.LearnerData = new LearnerData
                    {
                        Standard = certData.Standard,
                        Learner = certData.Learner,
                        LearningDetails = new Models.Response.Learners.LearningDetails { ProviderUkPrn = certData.LearningDetails.ProviderUkPrn, ProviderName = certData.LearningDetails.ProviderName, LearningStartDate = certData.LearningDetails.LearningStartDate}
                    };
                }
            }
        }

        public class MapStatusAction : IMappingAction<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearner>
        {
            public void Process(AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse source, GetLearner destination)
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
            public void Process(AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse source, GetLearner destination)
            {
                if (destination.Certificate is null) return;

                if (destination.Certificate.Status?.CurrentStatus == CertificateStatus.Deleted)
                {
                    destination.Certificate = null;
                }
                else if (destination.Certificate.Status?.CurrentStatus == CertificateStatus.Draft)
                {
                    if (!EpaOutcome.Pass.Equals(destination.EpaDetails?.LatestEpaOutcome, StringComparison.InvariantCultureIgnoreCase))
                    {
                        destination.Certificate = null;
                    }
                    else if (string.IsNullOrEmpty(destination.Certificate?.CertificateData?.LearningDetails?.OverallGrade)
                        || destination.Certificate?.CertificateData?.LearningDetails?.AchievementDate is null
                        || string.IsNullOrEmpty(destination.Certificate?.CertificateData?.PostalContact?.PostCode))
                    {
                        // Ensure we have a OverallGrade, AchievementDate and a PostalContact before seeing any Cert details
                        destination.Certificate = null;
                    }
                }
            }
        }

        public class CollapseNullsAction : IMappingAction<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearner>
        {
            public void Process(AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse source, GetLearner destination)
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
