using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Learners;
using SFA.DAS.AssessorService.Domain.Consts;

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
                .ForMember(dest => dest.EpaDetails, opt => opt.ResolveUsing(source => Mapper.Map<Domain.JsonData.CertificateData>(source.Certificate?.CertificateData)))
                .AfterMap<MapStatusAction>()
                .AfterMap<MapLearnerDataAction>()
                .ForAllOtherMembers(dest => dest.Ignore());
        }

        public class MapLearnerDataAction : IMappingAction<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearner>
        {
            public void Process(AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse source, GetLearner destination)
            {
                if (destination.Certificate?.CertificateData != null)
                {
                    destination.LearnerData = new LearnerData
                    {
                        Standard = destination.Certificate.CertificateData.Standard,
                        Learner = destination.Certificate.CertificateData.Learner,
                        LearningDetails = destination.Certificate.CertificateData.LearningDetails
                    };
                }
                else if (source.Learner != null)
                {
                    destination.LearnerData = new LearnerData
                    {
                        Standard = source.Learner.Standard is null ? null : new Standard { StandardCode = source.Learner.Standard.StandardId, StandardReference = source.Learner.Standard.ReferenceNumber, StandardName = source.Learner.Standard.Title, Level = source.Learner.Standard.StandardData?.Level ?? 0 },
                        Learner = new Learner { Uln = source.Learner.Uln, GivenNames = source.Learner.GivenNames, FamilyName = source.Learner.FamilyName },
                        LearningDetails = new LearningDetails { ProviderUkPrn = source.Learner.UkPrn, ProviderName = source.Learner.OrganisationName, LearningStartDate = source.Learner.LearnerStartDate }
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
                    // NOTE: This block of code allows us to deterime the completionStatus based on Certificate Status
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
    }
}
