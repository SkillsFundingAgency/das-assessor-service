using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class LearnerDetailRequest : IRequest<LearnerDetail>
    {
        public int StdCode { get; }
        public long Uln { get; }

        public LearnerDetailRequest(int stdCode, long uln)
        {
            StdCode = stdCode;
            Uln = uln;
        }
    }
}