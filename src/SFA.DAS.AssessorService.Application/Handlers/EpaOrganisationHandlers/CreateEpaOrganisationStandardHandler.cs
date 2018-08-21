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
    public class CreateEpaOrganisationStandardHandler : IRequestHandler<CreateEpaOrganisationStandardRequest, int>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<CreateEpaOrganisationStandardHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;

        public CreateEpaOrganisationStandardHandler(IRegisterRepository registerRepository, IEpaOrganisationValidator validator, ILogger<CreateEpaOrganisationStandardHandler> logger)
        {
            _registerRepository = registerRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<int> Handle(CreateEpaOrganisationStandardRequest request, CancellationToken cancellationToken)
        {
            var errorDetails = new StringBuilder();

            errorDetails.Append(_validator.CheckOrganisationIdIsPresentAndValid(request.OrganisationId));
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
            var organisationStandard = new EpaOrganisationStandard
            {
                OrganisationId = request.OrganisationId,
                StandardCode = request.StandardCode,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                DateStandardApprovedOnRegister = request.DateStandardApprovedOnRegister,
                Comments = request.Comments
            };
            return organisationStandard;
        }
    }
}
