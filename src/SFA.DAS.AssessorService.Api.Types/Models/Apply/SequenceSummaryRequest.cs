using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class SequenceSummaryRequest : IRequest<List<SequenceSummary>>
    {
        public string UserId { get; }

        public SequenceSummaryRequest(string userId)
        {
            UserId = userId;
        }
    }
}