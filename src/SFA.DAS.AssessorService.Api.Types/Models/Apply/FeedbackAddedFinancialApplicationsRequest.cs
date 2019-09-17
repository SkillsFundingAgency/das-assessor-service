using MediatR;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class FeedbackAddedFinancialApplicationsRequest : IRequest<List<FinancialApplicationSummaryItem>>
    {
    }
}
