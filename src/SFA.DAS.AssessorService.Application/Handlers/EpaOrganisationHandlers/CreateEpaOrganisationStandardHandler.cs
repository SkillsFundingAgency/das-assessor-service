using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class CreateEpaOrganisationStandardHandler : IRequestHandler<CreateEpaOrganisationStandardRequest, string>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<CreateEpaOrganisationStandardHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public CreateEpaOrganisationStandardHandler(IRegisterRepository registerRepository, IEpaOrganisationValidator validator, ILogger<CreateEpaOrganisationStandardHandler> logger, ISpecialCharacterCleanserService cleanser)
        {
            _registerRepository = registerRepository;
            _logger = logger;
            _cleanser = cleanser;
            _validator = validator;
        }

        public async Task<string> Handle(CreateEpaOrganisationStandardRequest request, CancellationToken cancellationToken)
        {
            var errorDetails = new StringBuilder();
            ProcessRequestFieldsForSpecialCharacters(request);
            errorDetails.Append(_validator.CheckOrganisationIdIsPresentAndValid(request.OrganisationId));
            errorDetails.Append(_validator.CheckIfContactIdIsEmptyOrValid(request.ContactId, request.OrganisationId));
            if (errorDetails.Length > 0)
            {
                _logger.LogError(errorDetails.ToString());
                throw new BadRequestException(errorDetails.ToString());
            }

            errorDetails.Append(_validator.CheckIfOrganisationNotFound(request.OrganisationId));
            ThrowNotFoundExceptionIfErrorPresent(errorDetails);

            errorDetails.Append(_validator.CheckIfStandardNotFound(request.StandardCode));
            ThrowNotFoundExceptionIfErrorPresent(errorDetails);

            errorDetails.Append(
                _validator.CheckIfOrganisationStandardAlreadyExists(request.OrganisationId, request.StandardCode));

            ThrowAlreadyExistsExceptionIfErrorPresent(errorDetails);

            var organisationStandard = MapOrganisationStandardRequestToOrganisationStandard(request);
           
            return await _registerRepository.CreateEpaOrganisationStandard(organisationStandard);
        }

        private void ProcessRequestFieldsForSpecialCharacters(CreateEpaOrganisationStandardRequest request)
        {
            request.OrganisationId = _cleanser.CleanseStringForSpecialCharacters(request.OrganisationId?.Trim());           
            request.Comments = _cleanser.CleanseStringForSpecialCharacters(request.Comments?.Trim());
            request.ContactId = request.ContactId?.Trim();
        }

        private void ThrowAlreadyExistsExceptionIfErrorPresent(StringBuilder errorDetails)
        {
            if (errorDetails.Length == 0) return;
            _logger.LogError(errorDetails.ToString());
            throw new AlreadyExistsException(errorDetails.ToString());
        }

        private void ThrowNotFoundExceptionIfErrorPresent(StringBuilder errorDetails)
        {
            if (errorDetails.Length == 0) return;
            _logger.LogError(errorDetails.ToString());
            throw new NotFound(errorDetails.ToString());
        }

        private static  EpaOrganisationStandard MapOrganisationStandardRequestToOrganisationStandard(CreateEpaOrganisationStandardRequest request)
        {
            Guid? contactId = null;
            if (Guid.TryParse(request.ContactId, out Guid contactIdGuid))
                contactId = contactIdGuid;

            var organisationStandard = new EpaOrganisationStandard
            {
                OrganisationId = request.OrganisationId,
                StandardCode = request.StandardCode,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                DateStandardApprovedOnRegister = null,
                Comments = request.Comments,
                ContactId = contactId
            };
            return organisationStandard;
        }
    }
}
