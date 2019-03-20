using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetCollatedStandardRequest : IRequest<StandardCollation>
    {
        public int? StandardId;
        public string ReferenceNumber;
    }
}
