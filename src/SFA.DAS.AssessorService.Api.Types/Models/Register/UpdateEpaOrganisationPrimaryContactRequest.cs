using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class UpdateEpaOrganisationPrimaryContactRequest : IRequest<bool>
    {
        public Guid PrimaryContactId { get; set; }
        public string OrganisationId { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
