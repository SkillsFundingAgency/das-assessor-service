using AutoMapper;
using MediatR;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Interfaces;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class UpdateEpaOrganisationPhoneNumberHandler : IRequestHandler<UpdateEpaOrganisationPhoneNumberRequest, List<ContactResponse>>
    { 
        private readonly IMediator _mediator;

        public UpdateEpaOrganisationPhoneNumberHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<List<ContactResponse>> Handle(UpdateEpaOrganisationPhoneNumberRequest request, CancellationToken cancellationToken)
        {
            var organisation = await _mediator.Send(new GetAssessmentOrganisationRequest { OrganisationId = request.OrganisationId });

            var updatedByContact = request.UpdatedBy.HasValue
                                ? await _mediator.Send(new GetEpaContactRequest { ContactId = request.UpdatedBy.Value })
                                : null;

            var updateEpaOrganisationRequest = Mapper.Map<UpdateEpaOrganisationRequest>(organisation);
            updateEpaOrganisationRequest.PhoneNumber = request.PhoneNumber;
            updateEpaOrganisationRequest.UpdatedBy = updatedByContact?.DisplayName ?? "Unknown";

            await _mediator.Send(updateEpaOrganisationRequest);

            return await _mediator.Send(new SendOrganisationDetailsAmendedEmailRequest
                {
                    OrganisationId = request.OrganisationId,
                    PropertyChanged = "Contact phone number",
                    ValueAdded = request.PhoneNumber,
                    Editor = updatedByContact?.DisplayName ?? "EFSA Staff"
            });
        }
    }
}
