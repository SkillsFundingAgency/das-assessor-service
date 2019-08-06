using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class UpdateEpaOrganisationWebsiteLinkRequest : IRequest<bool>
    {
        public string WebsiteLink { get; set; }
        public string OrganisationId { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
