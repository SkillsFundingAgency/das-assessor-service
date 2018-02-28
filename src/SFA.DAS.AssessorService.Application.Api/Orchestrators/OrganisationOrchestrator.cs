namespace SFA.DAS.AssessorService.Application.Api.Orchestrators
{
    using MediatR;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;

    public class OrganisationOrchestrator
    {
        private readonly IStringLocalizer<OrganisationOrchestrator> _localizer;
        private readonly IMediator _mediator;

        public OrganisationOrchestrator(IMediator mediator,
            IStringLocalizer<OrganisationOrchestrator> localizer,
            ILogger<OrganisationOrchestrator> logger)
        {
            _mediator = mediator;
            _localizer = localizer;
        }
    }
}