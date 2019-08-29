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
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;

        public UpdateEpaOrganisationEmailHandler(IContactQueryRepository contactQueryRepository, IMediator mediator)
        {
            _contactQueryRepository = contactQueryRepository;
            _mediator = mediator;
        }

        public async Task<List<ContactResponse>> Handle(UpdateEpaOrganisationEmailRequest request, CancellationToken cancellationToken)
        {
            var organisation = await _mediator.Send(new GetAssessmentOrganisationRequest { OrganisationId = request.OrganisationId });

            var updateEpaOrganisationRequest = Mapper.Map<UpdateEpaOrganisationRequest>(organisation);
            updateEpaOrganisationRequest.Email = request.Email;

            await _mediator.Send(updateEpaOrganisationRequest);

            var updatedBy = request.UpdatedBy.HasValue
                ? await _contactQueryRepository.GetContactById(request.UpdatedBy.Value)
                : null;

            return await _mediator.Send(new SendOrganisationDetailsAmendedEmailRequest
                {
                    OrganisationId = request.OrganisationId,
                    PropertyChanged = "Email address",
                    ValueAdded = request.Email,
                    Editor = updatedBy?.DisplayName ?? "EFSA Staff"
            });
        }
    }
}
