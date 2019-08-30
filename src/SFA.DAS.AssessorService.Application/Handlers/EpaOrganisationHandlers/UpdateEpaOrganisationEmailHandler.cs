using System.Threading;
using System.Threading.Tasks;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Interfaces;

using AutoMapper;
using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class UpdateEpaOrganisationEmailHandler : IRequestHandler<UpdateEpaOrganisationEmailRequest, List<ContactResponse>>
    { 
        private readonly IMediator _mediator;

        public UpdateEpaOrganisationEmailHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<List<ContactResponse>> Handle(UpdateEpaOrganisationEmailRequest request, CancellationToken cancellationToken)
        {
            var organisation = await _mediator.Send(new GetAssessmentOrganisationRequest { OrganisationId = request.OrganisationId });

            var updatedByContact = request.UpdatedBy.HasValue
                                ? await _mediator.Send(new GetEpaContactRequest { ContactId = request.UpdatedBy.Value })
                                : null;

            var updateEpaOrganisationRequest = Mapper.Map<UpdateEpaOrganisationRequest>(organisation);
            updateEpaOrganisationRequest.Email = request.Email;
            updateEpaOrganisationRequest.UpdatedBy = updatedByContact?.DisplayName ?? "Unknown";

            await _mediator.Send(updateEpaOrganisationRequest);

            return await _mediator.Send(new SendOrganisationDetailsAmendedEmailRequest
                {
                    OrganisationId = request.OrganisationId,
                    PropertyChanged = "Email address",
                    ValueAdded = request.Email,
                    Editor = updatedByContact?.DisplayName ?? "EFSA Staff"
            });
        }
    }
}
