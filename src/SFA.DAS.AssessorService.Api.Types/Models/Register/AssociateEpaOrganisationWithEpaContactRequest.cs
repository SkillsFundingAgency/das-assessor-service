using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class AssociateEpaOrganisationWithEpaContactRequest : IRequest<bool>
    {
        public Guid ContactId { get; set; }
        public string OrganisationId { get; set; }
        public string ContactStatus { get; set; }
        public bool MakePrimaryContact { get; set; }
        public bool AddDefaultRoles { get; set; }
        public bool AddDefaultPrivileges { get; set; }
    }
}
