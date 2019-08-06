using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class UpdateEpaOrganisationEmailRequest : IRequest<bool>
    {
        public string Email { get; set; }
        public string OrganisationId { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
