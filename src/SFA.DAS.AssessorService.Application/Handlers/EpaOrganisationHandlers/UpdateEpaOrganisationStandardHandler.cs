﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class UpdateEpaOrganisationStandardHandler : IRequestHandler<UpdateEpaOrganisationStandardRequest, string>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<UpdateEpaOrganisationStandardHandler> _logger;
        private readonly IEpaOrganisationValidator _validator;

        public UpdateEpaOrganisationStandardHandler(IRegisterRepository registerRepository, IEpaOrganisationValidator validator, ILogger<UpdateEpaOrganisationStandardHandler> logger)
        {
            _registerRepository = registerRepository;
            _validator = validator;
            _logger = logger;
        }


        public async Task<string> Handle(UpdateEpaOrganisationStandardRequest request, CancellationToken cancellationToken)
        {
            var errorDetails = new StringBuilder();
            errorDetails.Append(_validator.CheckIfOrganisationStandardDoesNotExist(request.OrganisationId, request.StandardCode));
            errorDetails.Append(_validator.CheckIfContactIdIsEmptyOrValid(request.ContactId,request.OrganisationId));

            if (errorDetails.Length > 0)
            {
                _logger.LogError(errorDetails.ToString());
                throw new BadRequestException(errorDetails.ToString());
            }

            var organisationStandard = MapOrganisationStandardRequestToOrganisationStandard(request);

            return await _registerRepository.UpdateEpaOrganisationStandard(organisationStandard);
        }

        private static EpaOrganisationStandard MapOrganisationStandardRequestToOrganisationStandard(UpdateEpaOrganisationStandardRequest request)
        {
            Guid? contactId = null;
            if (!string.IsNullOrEmpty(request.ContactId)  && Guid.TryParse(request.ContactId, out Guid contactIdGuid))
                contactId = contactIdGuid;

            var organisationStandard = new EpaOrganisationStandard
            {
                OrganisationId = request.OrganisationId,
                StandardCode = request.StandardCode,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                Comments = request.Comments,
                ContactId = contactId
            };
            return organisationStandard;
        }
    }
}
