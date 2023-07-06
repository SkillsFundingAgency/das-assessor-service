using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.OrganisationStandards;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationStandards
{
    public class AddOrganisationStandardRequestHandler : IRequestHandler<OrganisationStandardAddRequest, string>
    {
        private readonly IMediator _mediator;
        private readonly IStandardService _standardService;

        public AddOrganisationStandardRequestHandler(IMediator mediator, IStandardService standardService)
        {
            _mediator = mediator;
            _standardService = standardService;
        }

        public async Task<string> Handle(OrganisationStandardAddRequest request, CancellationToken cancellationToken)
        {
            var standard = (await _standardService.GetStandardVersionsByIFateReferenceNumber(request.StandardReference))
                .FirstOrDefault();

            if(standard == null)
            {
                throw new ArgumentException($"The {request.StandardReference} cannot be found.");
            }

            var createEpaOrganisationStandardRequest = new CreateEpaOrganisationStandardRequest
            {
                StandardCode = standard.LarsCode,
                EffectiveFrom = DateTime.UtcNow.Date,
                OrganisationId = request.OrganisationId,
                StandardVersions = request.StandardVersions,
                StandardReference = request.StandardReference,
                DeliveryAreas = await GetDeliveryAreas(),
                StandardApplicationType = string.Empty,
                EffectiveTo = null,
                DateStandardApprovedOnRegister = DateTime.Now.Date,
                DeliveryAreasComments = string.Empty,
                Comments = string.Empty,
                ContactId = request.ContactId.ToString()
            };

            var result = await _mediator.Send(createEpaOrganisationStandardRequest, cancellationToken);

            await _mediator.Send(new SendAddStandardEmailRequest
            {
                ContactId = request.ContactId.ToString(),
                StandardReference = request.StandardReference,
                StandardVersions = request.StandardVersions
            }, cancellationToken);

            return result;
        }

        private async Task<List<int>> GetDeliveryAreas()
        {
            var areas = await _mediator.Send(new GetDeliveryAreasRequest());
            return areas.Select(a => a.Id).ToList();
        }
    }
}