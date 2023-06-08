using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class OrganisationStandardVersionOptInHandler : IRequestHandler<OrganisationStandardVersionOptInRequest, OrganisationStandardVersion>
    {
        private readonly IOrganisationStandardRepository _organisationStandardRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IStandardService _standardService;
        private readonly IMediator _mediator;
        private readonly ILogger<OrganisationStandardVersionOptInHandler> _logger;

        public OrganisationStandardVersionOptInHandler(IOrganisationStandardRepository organisationStandardRepository, 
            IContactQueryRepository contactQueryRepository, IMediator mediator,
            IStandardService standardService,
            ILogger<OrganisationStandardVersionOptInHandler> logger)
        {
            _organisationStandardRepository = organisationStandardRepository;
            _contactQueryRepository = contactQueryRepository;
            _standardService = standardService;
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
                    throw new ArgumentException($"Cannnot opt in to standard {request.StandardReference} as contact {request.ContactId} cannot be found", 
                        nameof(request.ContactId));
                }

                var organisationStandard = await _organisationStandardRepository.GetOrganisationStandardByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference);
                if (organisationStandard == null)
                {
                    throw new ArgumentException($"Cannnot opt in as standard {request.StandardReference} for EPAO {request.EndPointAssessorOrganisationId} cannot be found", 
                        nameof(request.EndPointAssessorOrganisationId));
                }

                var allVersions = await _standardService.GetStandardVersionsByIFateReferenceNumber(request.StandardReference);
                var optInVersion = allVersions.FirstOrDefault(x => x.Version.Equals(request.Version, StringComparison.InvariantCultureIgnoreCase));
                if (optInVersion == null)
                {
                    throw new ArgumentException($"Cannnot opt in as standard {request.StandardReference} version {request.Version} cannot be found",
                        nameof(request.Version));
                }

                var existingVersion = await _organisationStandardRepository.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(organisationStandard.Id, request.Version);
                var newComment = $"Opted in by EPAO {contact.Email} at {DateTime.Now}";

                var entity = new Domain.Entities.OrganisationStandardVersion
                {
                    StandardUId = optInVersion.StandardUId,
                    Version = request.Version,
                    OrganisationStandardId = organisationStandard.Id,
                    EffectiveFrom = request.EffectiveFrom,
                    EffectiveTo = request.EffectiveTo,
                    DateVersionApproved = null,
                    Comments = existingVersion == null
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
                });

                return organisationStandardVersion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to opt-in standard version {request.StandardReference} {request.Version} for Organisation {request.EndPointAssessorOrganisationId}");
                throw;
            }
        }
    }
}