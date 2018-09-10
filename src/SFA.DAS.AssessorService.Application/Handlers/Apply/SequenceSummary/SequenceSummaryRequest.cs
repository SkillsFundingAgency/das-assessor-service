using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.SequenceSummary
{
    public class SequenceSummaryRequest : IRequest<List<SequenceSummary>>
    {
        public int UserId { get; }

        public SequenceSummaryRequest(int userId)
        {
            UserId = userId;
        }
    }
}