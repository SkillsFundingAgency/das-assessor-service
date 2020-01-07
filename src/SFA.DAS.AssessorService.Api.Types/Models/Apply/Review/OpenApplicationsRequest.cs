using MediatR;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Review
{
    public class OpenApplicationsRequest : IRequest<List<ApplicationSummaryItem>>
    {
        public int SequenceNo { get; }

        public OpenApplicationsRequest(int sequenceNo)
        {
            SequenceNo = sequenceNo;
        }
    }
}
