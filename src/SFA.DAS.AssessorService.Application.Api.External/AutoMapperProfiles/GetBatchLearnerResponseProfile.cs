using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Learners;

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
                .ForMember(dest => dest.Status, opt => opt.ResolveUsing(source => new Models.Response.Learners.Status { CurrentStatus = "1" })) // TODO: Figure out what should be here
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
    }
}
