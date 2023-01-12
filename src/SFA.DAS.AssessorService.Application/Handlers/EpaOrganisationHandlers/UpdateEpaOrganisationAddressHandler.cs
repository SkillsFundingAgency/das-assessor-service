using AutoMapper;
using MediatR;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Interfaces;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class UpdateEpaOrganisationAddressHandler : IRequestHandler<UpdateEpaOrganisationAddressRequest, List<ContactResponse>>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;

        public UpdateEpaOrganisationAddressHandler(IContactQueryRepository contactQueryRepository, IMediator mediator)
        {
            _contactQueryRepository = contactQueryRepository;
            _mediator = mediator;
        }

        public async Task<List<ContactResponse>> Handle(UpdateEpaOrganisationAddressRequest request, CancellationToken cancellationToken)
        {
            var organisation = await _mediator.Send(new GetAssessmentOrganisationRequest { OrganisationId = request.OrganisationId });

            var updateEpaOrganisationRequest = Mapper.Map<UpdateEpaOrganisationRequest>(organisation);
            updateEpaOrganisationRequest.Address1 = request.AddressLine1;
            updateEpaOrganisationRequest.Address2 = request.AddressLine2;
            updateEpaOrganisationRequest.Address3 = request.AddressLine3;
            updateEpaOrganisationRequest.Address4 = request.AddressLine4;
            updateEpaOrganisationRequest.Postcode = request.Postcode;

            await _mediator.Send(updateEpaOrganisationRequest);

            var updatedBy = request.UpdatedBy.HasValue
                ? await _contactQueryRepository.GetContactById(request.UpdatedBy.Value)
                : null;

            string valueAdded = string.Join(", ",
                (new List<string>
                {
                    request.AddressLine1,
                    request.AddressLine2,
                    request.AddressLine3,
                    request.AddressLine4,
                    request.Postcode
                })
                .Where(p => !string.IsNullOrEmpty(p))
                .ToArray());

            return await _mediator.Send(new SendOrganisationDetailsAmendedEmailRequest
            {
                OrganisationId = request.OrganisationId,
                PropertyChanged = "Contact address",
                ValueAdded = valueAdded,
                Editor = updatedBy?.DisplayName ?? "EFSA Staff"
            });
        }
    }
}
