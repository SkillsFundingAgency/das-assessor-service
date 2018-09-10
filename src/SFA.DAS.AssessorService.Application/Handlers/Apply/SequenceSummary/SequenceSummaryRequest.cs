using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.SequenceSummary
{
    public class SequenceSummaryRequest : IRequest<List<SequenceSummary>>
    {
        private readonly int _userId;

        public SequenceSummaryRequest(int userId)
        {
            _userId = userId;
        }
    }
}