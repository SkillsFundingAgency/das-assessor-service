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
    public class UpdateEpaOrganisationStandardHandler : IRequestHandler<UpdateEpaOrganisationStandardRequest, int>
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


        public async Task<int> Handle(UpdateEpaOrganisationStandardRequest request, CancellationToken cancellationToken)
        {
            var errorDetails = new StringBuilder();
            errorDetails.Append(_validator.CheckIfOrganisationStandardDoesNotExist(request.OrganisationId, request.StandardCode));

            if (errorDetails.Length > 0)
            {
                _logger.LogError(errorDetails.ToString());
                throw new BadRequestException(errorDetails.ToString());
            }

            var organisationStandard = MapOrganisationStandardRequestToOrganisationStandard(request);

            return await _registerRepository.UpdateEpaOrganisationStandard(organisationStandard);
        }

        private static EpaOrganisation MapOrganisationRequestToOrganisation(UpdateEpaOrganisationRequest request)
        {
            var organisation = new EpaOrganisation
            {
                Name = request.Name.Trim(),
                OrganisationId = request.OrganisationId.Trim(),
                OrganisationTypeId = request.OrganisationTypeId,
                Ukprn = request.Ukprn,
                OrganisationData = new OrganisationData
                {
                    Address1 = request.Address1,
                    Address2 = request.Address2,
                    Address3 = request.Address3,
                    Address4 = request.Address4,
                    LegalName = request.LegalName,
                    Postcode = request.Postcode,
                    WebsiteLink = request.WebsiteLink
                }
            };

            return organisation;
        }

        private static EpaOrganisationStandard MapOrganisationStandardRequestToOrganisationStandard(UpdateEpaOrganisationStandardRequest request)
        {
            var organisationStandard = new EpaOrganisationStandard
            {
                OrganisationId = request.OrganisationId,
                StandardCode = request.StandardCode,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                DateStandardApprovedOnRegister = request.DateStandardApprovedOnRegister,
                Comments = request.Comments,
            };
            return organisationStandard;
        }
    }
}
