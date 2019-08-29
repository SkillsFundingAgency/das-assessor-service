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
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;

        public UpdateEpaOrganisationWebsiteLinkHandler(IContactQueryRepository contactQueryRepository, IMediator mediator)
        {
            _contactQueryRepository = contactQueryRepository;
            _mediator = mediator;
        }

        public async Task<List<ContactResponse>> Handle(UpdateEpaOrganisationWebsiteLinkRequest request, CancellationToken cancellationToken)
        {
            var organisation = await _mediator.Send(new GetAssessmentOrganisationRequest { OrganisationId = request.OrganisationId });

            var updateEpaOrganisationRequest = Mapper.Map<UpdateEpaOrganisationRequest>(organisation);
            updateEpaOrganisationRequest.WebsiteLink = request.WebsiteLink;

            await _mediator.Send(updateEpaOrganisationRequest);

            var updatedBy = request.UpdatedBy.HasValue
                ? await _contactQueryRepository.GetContactById(request.UpdatedBy.Value)
                : null;

            return await _mediator.Send(new SendOrganisationDetailsAmendedEmailRequest
                {
                    OrganisationId = request.OrganisationId,
                    PropertyChanged = "Website address",
                    ValueAdded = request.WebsiteLink,
                    Editor = updatedBy?.DisplayName ?? "EFSA Staff"
            });
        }
    }
}
