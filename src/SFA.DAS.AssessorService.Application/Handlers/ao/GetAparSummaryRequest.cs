using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetAparSummaryRequest : IRequest<List<AparSummaryItem>>
    {
    }
}
