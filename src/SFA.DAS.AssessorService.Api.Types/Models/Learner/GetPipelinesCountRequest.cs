using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetPipelinesCountRequest : IRequest<int>
    {
        public GetPipelinesCountRequest(string epaoId, int? standardCode)
        {
            EpaoId = epaoId;
            StandardCode = standardCode;
        }

        public string EpaoId { get; }
        public int? StandardCode { get; }
    }
}