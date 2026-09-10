using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class OrganisationStandardVersionOptInHandler : IRequestHandler<OrganisationStandardVersionOptInRequest, OrganisationStandardVersion>
    {
        private readonly IOrganisationStandardRepository _organisationStandardRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IStandardService _standardService;
        private readonly IOrganisationStandardSelector _organisationStandardSelector;
        private readonly IMediator _mediator;
        private readonly ILogger<OrganisationStandardVersionOptInHandler> _logger;

        public OrganisationStandardVersionOptInHandler(IOrganisationStandardRepository organisationStandardRepository,
            IContactQueryRepository contactQueryRepository, IMediator mediator,
            IStandardService standardService,
            IOrganisationStandardSelector organisationStandardSelector,
            ILogger<OrganisationStandardVersionOptInHandler> logger)
        {
            _organisationStandardRepository = organisationStandardRepository;
            _contactQueryRepository = contactQueryRepository;
            _standardService = standardService;
            _organisationStandardSelector = organisationStandardSelector;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<OrganisationStandardVersion> Handle(OrganisationStandardVersionOptInRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var contact = await _contactQueryRepository.GetContactById(request.ContactId);
                if (contact == null)
                {
                    throw new NotFoundException($"Cannot opt in to StandardReference {request.StandardReference} as ContactId {request.ContactId} cannot be found");
                }

                var optInVersion = await _standardService.GetStandardVersionById(request.StandardReference, request.Version);
                if (optInVersion == null)
                {
                    throw new NotFoundException($"Cannot opt in as StandardReference {request.StandardReference} Version {request.Version} cannot be found");
                }

                var organisationStandards = await _organisationStandardRepository
                    .GetOrganisationStandardsByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference);
                var selection = _organisationStandardSelector.Select(organisationStandards, optInVersion.LarsCode);

                if (selection.Result == OrganisationStandardSelectionResult.NotFound)
                {
                    throw new NotFoundException($"Cannot opt in as StandardReference {request.StandardReference} for EndPointAssessorOrganisationId {request.EndPointAssessorOrganisationId} cannot be found");
                }

                if (selection.Result == OrganisationStandardSelectionResult.Ambiguous)
                {
                    throw new NotFoundException($"Cannot opt in as StandardReference {request.StandardReference} LarsCode {optInVersion.LarsCode} for EndPointAssessorOrganisationId {request.EndPointAssessorOrganisationId} is ambiguous ({selection.TotalRowCount} records found, {selection.MatchingLarsCodeCount} matched the LarsCode)");
                }

                var organisationStandard = selection.OrganisationStandard;

                var existingVersion = await _organisationStandardRepository.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(organisationStandard.Id, request.Version);
                var newComment = $"Opted in by EPAO {contact.Email} at {request.OptInRequestedAt}";

                var entity = new Domain.Entities.OrganisationStandardVersion
                {
                    StandardUId = optInVersion.StandardUId,
                    Version = request.Version,
                    OrganisationStandardId = organisationStandard.Id,
                    EffectiveFrom = request.EffectiveFrom,
                    EffectiveTo = request.EffectiveTo,
                    DateVersionApproved = request.OptInRequestedAt,
                    Comments = string.IsNullOrEmpty(existingVersion?.Comments)
                        ? newComment
                        : existingVersion.Comments + ";" + newComment,
                    Status = OrganisationStatus.Live
                };

                if (existingVersion != null)
                {
                    await _organisationStandardRepository.UpdateOrganisationStandardVersion(entity);
                }
                else
                { 
                    await _organisationStandardRepository.CreateOrganisationStandardVersion(entity);
                }

                var organisationStandardVersion = (OrganisationStandardVersion)entity;

                await _mediator.Send(new SendOptInStandardVersionEmailRequest
                {
                    ContactId = request.ContactId,
                    StandardReference = request.StandardReference,
                    Version = request.Version,
                }, cancellationToken);

                return organisationStandardVersion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to opt-in StandardReference {request.StandardReference} Version {request.Version} for EndPointAssessorOrganisationId {request.EndPointAssessorOrganisationId}");
                throw;
            }
        }
    }
}