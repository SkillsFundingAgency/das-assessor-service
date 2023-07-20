using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetAparSummaryRequest : IRequest<List<AssessmentOrganisationListSummary>>
    {
    }
}
