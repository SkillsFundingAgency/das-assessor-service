using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch
{
    public class GetFrameworkLearnerRequest: IRequest<GetFrameworkLearnerResponse>
    {
        public Guid Id { get; set; }
        public bool AllLogs { get; set; }

        public GetFrameworkLearnerRequest(Guid id, bool allLogs = false)
        {
            Id = id;
            AllLogs = allLogs;
        }
    }
}
