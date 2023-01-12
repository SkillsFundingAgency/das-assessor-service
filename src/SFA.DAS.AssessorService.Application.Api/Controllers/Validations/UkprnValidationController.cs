using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Infrastructure;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Validations
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    public class UkprnValidationController : Controller
    {
        private readonly IRoatpApiClient _roatpApiClient;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public UkprnValidationController(IRoatpApiClient roatpApiClient, IOrganisationQueryRepository organisationQueryRepository)
        {
            _roatpApiClient = roatpApiClient;
            _organisationQueryRepository = organisationQueryRepository;
        }

        [HttpGet("Validations/UkPrn/{id}")]
        public async Task<ActionResult<ApiValidationResult>> ValidateUkPrn(Guid id, int q)
        {
            // false if on epao register already.
            var epaOrg = await _organisationQueryRepository.GetByUkPrn(q);
            if (epaOrg != null)
            {
                return new ApiValidationResult { IsValid = false, ErrorMessages = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("", "UKPRN provided already in use") } };
            }

            // starts with 9, allow
            if (q.ToString().StartsWith("9")) return new ApiValidationResult { IsValid = true };

            var ukrlpResult = await _roatpApiClient.SearchOrganisationInUkrlp(q);
            if (ukrlpResult == null || !ukrlpResult.Any())
            {
                return new ApiValidationResult { IsValid = false, ErrorMessages = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("", "UKPRN is unknown") } };
            }

            return new ApiValidationResult { IsValid = true };
        }
    }
}