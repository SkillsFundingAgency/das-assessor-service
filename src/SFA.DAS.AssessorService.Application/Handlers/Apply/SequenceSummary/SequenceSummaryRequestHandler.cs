using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.SequenceSummary
{
    public class SequenceSummaryRequestHandler : IRequestHandler<SequenceSummaryRequest, List<SequenceSummary>>
    {
        public async Task<List<SequenceSummary>> Handle(SequenceSummaryRequest request, CancellationToken cancellationToken)
        {
            return new List<SequenceSummary>()
            {
                new SequenceSummary() {SequenceId = "S-01", Title = "1. Enter your company details", Active = true, Completed = true},
                new SequenceSummary() {SequenceId = "S-02", Title = "2. Make declaration", Active = true, Completed = false},
                new SequenceSummary() {SequenceId = "S-03", Title = "3. Add, edit and complete financial health assessment", Active = false, Completed = false},
                new SequenceSummary() {SequenceId = "S-04", Title = "4. Add, edit and complete application to assess Financial Adviser", Active = false, Completed = false}
            };
        }
    }
}