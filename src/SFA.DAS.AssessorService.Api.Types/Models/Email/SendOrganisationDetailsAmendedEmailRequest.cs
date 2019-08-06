using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SendOrganisationDetailsAmendedEmailRequest : IRequest
    {
        public string OrganisationId { get; set; }
        public string PropertyChanged { get; set; }
        public string ValueAdded { get; set; }
        public string Editor { get; set; }
    }
}
