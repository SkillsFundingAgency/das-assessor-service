using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Exceptions;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Api.Orchestrators
{
    public class OrganisationOrchestrator
    {
        private readonly IStringLocalizer<OrganisationOrchestrator> _localizer;
        private readonly IMediator _mediator;

        public OrganisationOrchestrator(IMediator mediator,
            IStringLocalizer<OrganisationOrchestrator> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        public async Task<Organisation> CreateOrganisation(
            [FromBody] CreateOrganisationRequest organisationCreateViewModel)
        {
            var organisation = await _mediator.Send(organisationCreateViewModel);
            return organisation;
        }


        public async Task UpdateOrganisation(
            [FromBody] UpdateOrganisationRequest organisationUpdateViewModel)
        {
            var organisation = await _mediator.Send(organisationUpdateViewModel);
        }

        public async Task DeleteOrganisation(string endPointAssessorOrganisationId)
        {
            try
            {
                var deleteOrganisationRequest = new DeleteOrganisationRequest
                {
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId
                };

                await _mediator.Send(deleteOrganisationRequest);
            }
            catch (NotFound)
            {
                throw new ResourceNotFoundException();
            }
        }
    }
}