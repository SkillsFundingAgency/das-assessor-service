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
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;

        public UpdateEpaOrganisationPhoneNumberHandler(IContactQueryRepository contactQueryRepository, IMediator mediator)
        {
            _contactQueryRepository = contactQueryRepository;
            _mediator = mediator;
        }

        public async Task<List<ContactResponse>> Handle(UpdateEpaOrganisationPhoneNumberRequest request, CancellationToken cancellationToken)
        {
            var organisation = await _mediator.Send(new GetAssessmentOrganisationRequest { OrganisationId = request.OrganisationId });

            var updateEpaOrganisationRequest = Mapper.Map<UpdateEpaOrganisationRequest>(organisation);
            updateEpaOrganisationRequest.PhoneNumber = request.PhoneNumber;

            await _mediator.Send(updateEpaOrganisationRequest);

            var updatedBy = request.UpdatedBy.HasValue
                ? await _contactQueryRepository.GetContactById(request.UpdatedBy.Value)
                : null;

            return await _mediator.Send(new SendOrganisationDetailsAmendedEmailRequest
                {
                    OrganisationId = request.OrganisationId,
                    PropertyChanged = "Contact phone number",
                    ValueAdded = request.PhoneNumber,
                    Editor = updatedBy?.DisplayName ?? "EFSA Staff"
            });
        }
    }
}
