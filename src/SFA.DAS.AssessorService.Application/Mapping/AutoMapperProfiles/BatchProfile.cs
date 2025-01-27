using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;


namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class CreateBatchLogRequestProfile : Profile
    {
        public CreateBatchLogRequestProfile()
        {
            CreateMap<CreateBatchLogRequest, BatchLog>();

        }
    }

    public class BatchDataProfile : Profile
    {
        public BatchDataProfile()
        {
            CreateMap<BatchData, BatchDataResponse>();
        }
    }

    public class BatchLogProfile : Profile
    {
        public BatchLogProfile()
        {
            CreateMap<BatchLog, BatchLogResponse>()
                .ForMember(q => q.BatchData, opts => { opts.MapFrom(q => q.BatchData); });

        }
    }
}
