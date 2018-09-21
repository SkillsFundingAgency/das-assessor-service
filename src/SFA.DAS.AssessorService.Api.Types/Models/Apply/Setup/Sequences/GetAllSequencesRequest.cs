using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup.Sequences
{
    public class GetAllSequencesRequest : IRequest<List<SequenceSummary>>
    {
        
    }
}