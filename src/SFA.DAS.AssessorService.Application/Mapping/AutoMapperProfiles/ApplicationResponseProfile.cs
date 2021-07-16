using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Mapping.CustomResolvers;
using SFA.DAS.AssessorService.Domain.DTOs;

namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class ApplicationResponseProfile : Profile
    {
        public ApplicationResponseProfile()
        {
            // Request going to Int API
            CreateMap<ApplySummary, ApplicationResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(source => source.ApplicationId))
                .ForMember(dest => dest.OrganisationId, opt => opt.MapFrom(source => source.OrganisationId))
                .ForMember(dest => dest.EndPointAssessorName, opt => opt.MapFrom(source => source.EndPointAssessorName))
                .ForMember(dest => dest.FinancialGrade, opt => opt.MapFrom(source => source.FinancialGrade))
                .ForMember(dest => dest.ApplicationStatus, opt => opt.MapFrom(source => source.ApplicationStatus))
                .ForMember(dest => dest.ReviewStatus, opt => opt.MapFrom(source => source.ReviewStatus))
                .ForMember(dest => dest.FinancialReviewStatus, opt => opt.MapFrom(source => source.FinancialReviewStatus))
                .ForMember(dest => dest.ApplyData, opt => opt.MapFrom(source => source.ApplyData))
                .ForMember(dest => dest.StandardCode, opt => opt.MapFrom(source => source.StandardCode))
                .ForMember(dest => dest.StandardReference, opt => opt.MapFrom(source => source.StandardReference))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(source => source.CreatedBy))
                .ForMember(dest => dest.ContactName, opt => opt.MapFrom(source => source.CreatedByName))
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(source => source.CreatedByEmail))
                .ForMember(dest => dest.ApplicationType, opts => { opts.ResolveUsing<ApplicationTypeResolver>(); })
                .ForMember(dest => dest.StandardApplicationType, opt => opt.MapFrom(source => source.StandardApplicationType))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
