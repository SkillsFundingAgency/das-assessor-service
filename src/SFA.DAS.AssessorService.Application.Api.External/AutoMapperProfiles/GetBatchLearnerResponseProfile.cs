using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Learners;
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
                else
                {
                    destination.LearnerData = new LearnerData
                    {
                        Standard = source.Standard is null ? null : new Standard { StandardCode = source.Standard.StandardId, StandardReference = source.Standard.ReferenceNumber, StandardName = source.Standard.Title, Level = source.Standard.StandardData?.Level ?? 0 },
                        Learner = source.Ilr is null ? null : new Learner { Uln = source.Ilr.Uln, GivenNames = source.Ilr.GivenNames, FamilyName = source.Ilr.FamilyName },
                        LearningDetails = source.Provider is null ? null : new LearningDetails { ProviderUkPrn = (int)source.Provider.Ukprn, ProviderName = source.Provider.ProviderName, LearningStartDate = source.Ilr?.LearnStartDate ?? DateTime.UtcNow.Date }
                    };
                }
            }
        }
    }
}
