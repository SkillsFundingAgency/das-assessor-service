using AutoMapper;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;

namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class ImportApprovalsProfile : Profile
    {
        public ImportApprovalsProfile()
        {
            CreateMap<Learner, Domain.Entities.ApprovalsExtract>();
        }
    }
}
