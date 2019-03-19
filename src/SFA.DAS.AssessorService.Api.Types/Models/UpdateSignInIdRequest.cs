using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateSignInIdRequest : IRequest
    {
        public Guid SignInId { get; }
        public Guid ContactId { get; }

        public UpdateSignInIdRequest(Guid signInId, Guid contactId)
        {
            SignInId = signInId;
            ContactId = contactId;
        }
    }
}
