using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SendOrganisationDetailsAmendedEmailRequest : IRequest<List<ContactResponse>>
    {
        public string OrganisationId { get; set; }
        public string PropertyChanged { get; set; }
        public string ValueAdded { get; set; }
        public string Editor { get; set; }
    }
}
