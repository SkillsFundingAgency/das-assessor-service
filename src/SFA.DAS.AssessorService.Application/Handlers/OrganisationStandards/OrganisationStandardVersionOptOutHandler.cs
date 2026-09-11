using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class OrganisationStandardVersionOptOutHandler : IRequestHandler<OrganisationStandardVersionOptOutRequest, OrganisationStandardVersion>
    {
        private readonly IOrganisationStandardRepository _organisationStandardRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IStandardService _standardService;
        private readonly IOrganisationStandardSelector _organisationStandardSelector;
        private readonly IMediator _mediator;
        private readonly ILogger<OrganisationStandardVersionOptOutHandler> _logger;

        public OrganisationStandardVersionOptOutHandler(IOrganisationStandardRepository organisationStandardRepository,
            IContactQueryRepository contactQueryRepository, IMediator mediator,
            IStandardService standardService,
            IOrganisationStandardSelector organisationStandardSelector,
            ILogger<OrganisationStandardVersionOptOutHandler> logger)
        {
            _organisationStandardRepository = organisationStandardRepository;
            _contactQueryRepository = contactQueryRepository;
            _standardService = standardService;
            _organisationStandardSelector = organisationStandardSelector;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<OrganisationStandardVersion> Handle(OrganisationStandardVersionOptOutRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var contact = await _contactQueryRepository.GetContactById(request.ContactId);
                if (contact == null)
                {
                    throw new NotFoundException($"Cannot opt out to StandardReference {request.StandardReference} as ContactId {request.ContactId} cannot be found");
                }

                var optOutVersion = await _standardService.GetStandardVersionById(request.StandardReference, request.Version);
                if (optOutVersion == null)
                {
                    throw new NotFoundException($"Cannot opt out as StandardReference {request.StandardReference} Version {request.Version} cannot be found");
                }

                var organisationStandards = await _organisationStandardRepository
                    .GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference);
                var selection = _organisationStandardSelector.Select(organisationStandards, optOutVersion.LarsCode);

                if (selection.Result == OrganisationStandardSelectionResult.NotFound
                    || selection.Result == OrganisationStandardSelectionResult.RequiresNewRecord)
                {
                    throw new NotFoundException($"Cannot opt out as StandardReference {request.StandardReference} LarsCode {optOutVersion.LarsCode} for EndPointAssessorOrganisationId {request.EndPointAssessorOrganisationId} cannot be found ({selection.TotalRowCount} existing OrganisationStandard records)");
                }

                var organisationStandard = selection.OrganisationStandard;

                var existingVersion = await _organisationStandardRepository.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(organisationStandard.Id, request.Version);
                if(existingVersion == null)
                {
                    throw new NotFoundException($"Cannot opt out as StandardReference {request.StandardReference} Version {request.Version} for {request.EndPointAssessorOrganisationId} cannot be found");
                }

                var newComment = $"Opted out by EPAO {contact.Email} at {request.OptOutRequestedAt}";

                var entity = new Domain.Entities.OrganisationStandardVersion
                {
                    StandardUId = existingVersion.StandardUId,
                    Version = request.Version,
                    OrganisationStandardId = organisationStandard.Id,
                    EffectiveFrom = request.EffectiveFrom,
                    EffectiveTo = request.EffectiveTo,
                    DateVersionApproved = request.OptOutRequestedAt,
                    Comments = string.IsNullOrEmpty(existingVersion?.Comments)
                        ? newComment
                        : existingVersion.Comments + ";" + newComment,
                    Status = OrganisationStatus.Live
                };

                await _organisationStandardRepository.UpdateOrganisationStandardVersion(entity);

                var organisationStandardVersion = (OrganisationStandardVersion)entity;

                await _mediator.Send(new SendOptOutStandardVersionEmailRequest
                {
                    ContactId = request.ContactId,
                    StandardReference = request.StandardReference,
                    Version = request.Version,
                }, cancellationToken);

                return organisationStandardVersion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to opt-out StandardReference {request.StandardReference} Version {request.Version} for EndPointAssessorOrganisationId {request.EndPointAssessorOrganisationId}");
                throw;
            }
        }
    }
}