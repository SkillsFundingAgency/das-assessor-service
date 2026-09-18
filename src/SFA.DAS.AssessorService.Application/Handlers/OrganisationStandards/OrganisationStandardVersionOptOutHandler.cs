using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Exceptions;
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
        private readonly IMediator _mediator;
        private readonly IStandardService _standardService;
        private readonly ILogger<OrganisationStandardVersionOptOutHandler> _logger;

        public OrganisationStandardVersionOptOutHandler(IOrganisationStandardRepository organisationStandardRepository, 
            IContactQueryRepository contactQueryRepository, IMediator mediator,
            IStandardService standardService,
            ILogger<OrganisationStandardVersionOptOutHandler> logger)
        {
            _organisationStandardRepository = organisationStandardRepository;
            _contactQueryRepository = contactQueryRepository;
            _mediator = mediator;
            _standardService = standardService;
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

                var allVersions = await _standardService.GetStandardVersionsByIFateReferenceNumber(request.StandardReference);
                var optOutVersion = allVersions.FirstOrDefault(x => x.Version.Equals(request.Version, StringComparison.InvariantCultureIgnoreCase));
                if (optOutVersion == null)
                {
                    throw new NotFoundException($"Cannot opt out as StandardReference {request.StandardReference} Version {request.Version} cannot be found");
                }

                var organisationStandards = await _organisationStandardRepository.GetOrganisationStandardsByOrganisationIdAndStandardReference(
                    request.EndPointAssessorOrganisationId,
                    request.StandardReference);

                if (organisationStandards.Count == 0)
                {
                    throw new NotFoundException(
                        $"Cannot opt out as StandardReference {request.StandardReference} " +
                        $"for EndPointAssessorOrganisationId " +
                        $"{request.EndPointAssessorOrganisationId} cannot be found");
                }

                Domain.Entities.OrganisationStandard organisationStandard;

                if (organisationStandards.Count == 1)
                {
                    organisationStandard = organisationStandards.Single();
                }
                else
                {
                    var matchingOrganisationStandards = organisationStandards
                        .Where(x => x.StandardCode == optOutVersion.LarsCode)
                        .ToList();

                    if (matchingOrganisationStandards.Count != 1)
                    {
                        throw new BadRequestException(
                            $"Cannot uniquely identify OrganisationStandard for " +
                            $"{request.EndPointAssessorOrganisationId}, " +
                            $"{request.StandardReference} and StandardCode " +
                            $"{optOutVersion.LarsCode}");
                    }

                    organisationStandard = matchingOrganisationStandards.Single();
                }

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