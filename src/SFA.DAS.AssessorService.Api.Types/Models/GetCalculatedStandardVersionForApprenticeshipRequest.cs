using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetCalculatedStandardVersionForApprenticeshipRequest : IRequest<Standard>
    {
        /// <summary>
        /// Can be StandardUId, LarsCode or IFateReferenceNumber
        /// </summary>
        public string StandardId { get; set; }
        public long Uln { get; set; }
    }
}
