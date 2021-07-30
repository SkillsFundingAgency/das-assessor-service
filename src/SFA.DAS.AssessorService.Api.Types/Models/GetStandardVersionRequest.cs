using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetStandardVersionRequest : IRequest<Standard>
    {
        /// <summary>
        /// Can be StandardUId, LarsCode or IFateReferenceNumber
        /// </summary>
        public string StandardId { get; set; }
        public string Version { get; set; }
    }
}
