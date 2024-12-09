using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateGovUkIdentifierRequest : IRequest<Unit>
    {
        public Guid ContactId { get; }
        public string GovIdentifier { get; }

        public UpdateGovUkIdentifierRequest(Guid contactId, string govIdentifier = null)
        {
            ContactId = contactId;
            GovIdentifier = govIdentifier;
        }
    }
}
