using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Interfaces;

using AutoMapper;
using MediatR;


namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class UpdateEpaOrganisationWebsiteLinkHandler : IRequestHandler<UpdateEpaOrganisationWebsiteLinkRequest, List<ContactResponse>>
    { 
        private readonly IMediator _mediator;

        public UpdateEpaOrganisationWebsiteLinkHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<List<ContactResponse>> Handle(UpdateEpaOrganisationWebsiteLinkRequest request, CancellationToken cancellationToken)
        {
            var organisation = await _mediator.Send(new GetAssessmentOrganisationRequest { OrganisationId = request.OrganisationId });

            var updatedByContact = request.UpdatedBy.HasValue
                                ? await _mediator.Send(new GetEpaContactRequest { ContactId = request.UpdatedBy.Value })
                                : null;

            var updateEpaOrganisationRequest = Mapper.Map<UpdateEpaOrganisationRequest>(organisation);
            updateEpaOrganisationRequest.WebsiteLink = request.WebsiteLink;
            updateEpaOrganisationRequest.UpdatedBy = updatedByContact?.DisplayName ?? "Unknown";

            await _mediator.Send(updateEpaOrganisationRequest);

            return await _mediator.Send(new SendOrganisationDetailsAmendedEmailRequest
                {
                    OrganisationId = request.OrganisationId,
                    PropertyChanged = "Website address",
                    ValueAdded = request.WebsiteLink,
                    Editor = updatedByContact?.DisplayName ?? "EFSA Staff"
            });
        }
    }
}
