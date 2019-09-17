using MediatR;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class ClosedFinancialApplicationsRequest : IRequest<List<FinancialApplicationSummaryItem>>
    {
    }
}
