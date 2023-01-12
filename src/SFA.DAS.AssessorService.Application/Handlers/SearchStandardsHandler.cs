using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class SearchStandardsHandler : IRequestHandler<SearchStandardsRequest, List<Standard>>
    {
        private readonly IStandardService _standardService;
        private readonly ILogger<SearchStandardsHandler> _logger;
        private readonly ISpecialCharacterCleanserService _cleanser;
        private readonly IEpaOrganisationValidator _validator;

        public SearchStandardsHandler(IStandardService standardService, ILogger<SearchStandardsHandler> logger, ISpecialCharacterCleanserService cleanser, IEpaOrganisationValidator validator)
        {
            _standardService = standardService;
            _logger = logger;
            _cleanser = cleanser;
            _validator = validator;
        }

        public async Task<List<Standard>> Handle(SearchStandardsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling Search Standards Request");

            var searchstring = _cleanser.UnescapeAndRemoveNonAlphanumericCharacters(request.SearchTerm);
            var validationResponse = _validator.ValidatorSearchStandardsRequest(new SearchStandardsValidationRequest { Searchstring = searchstring });
            if (!validationResponse.IsValid)
            {
                var message = validationResponse.Errors.Aggregate(string.Empty, (current, error) => current + error.ErrorMessage + "; ");
                _logger.LogError(message);
                if (validationResponse.Errors.Any(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()))
                {
                    throw new BadRequestException(message);
                }

                throw new Exception(message);
            }

            var isAnInt = int.TryParse(searchstring, out var intSearchString);

            var allStandards = await _standardService.GetLatestStandardVersions();

            return isAnInt
                ? allStandards.Where(x => x.LarsCode == intSearchString).ToList()
                : allStandards.Where(x =>
                    _cleanser.UnescapeAndRemoveNonAlphanumericCharacters(x.Title.ToLower()).Contains(searchstring.ToLower())).ToList();
        }
    }
}