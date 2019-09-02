using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{
    public class UpdateEpaOrganisationWebsiteLinkRequest : IRequest<List<ContactResponse>>
    {
        public string WebsiteLink { get; set; }
        public string OrganisationId { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
