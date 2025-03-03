using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch
{
    public class GetFrameworkLearnerRequest: IRequest<GetFrameworkLearnerResponse>
    {
        public Guid Id { get; set; }
    }
}
