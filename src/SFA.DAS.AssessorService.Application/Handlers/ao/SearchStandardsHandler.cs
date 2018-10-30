using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Web.Staff.Services;

namespace SFA.DAS.AssessorService.Application.Handlers
{
    public class SearchStandardsHandler: IRequestHandler<SearchStandardsRequest, List<StandardSummary>>
    {
        private readonly IStandardService _standardSearch;
        private readonly ILogger<SearchStandardsHandler> _logger;
        //private readonly IEpaOrganisationSearchValidator _validator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public SearchStandardsHandler(IStandardService standardSearch, ILogger<SearchStandardsHandler> logger, ISpecialCharacterCleanserService cleanser)
        {
            _standardSearch = standardSearch;
            _logger = logger;
            _cleanser = cleanser;
        }

        public async Task<List<StandardSummary>> Handle(SearchStandardsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling Search Standards Request");
            
            var searchstring = _cleanser.UnescapeAndRemoveNonAlphanumericCharacters(request.SearchTerm);
            
            var isAnInt = int.TryParse(searchstring, out _);
            if (!isAnInt && searchstring.Length < 2)
                throw new BadRequestException("The searchstring is too short to do a valid search");

            var allStandards = await _standardSearch.GetAllStandardSummaries();
            return isAnInt 
                ? allStandards.Where(x => x.Id == searchstring).ToList() 
                : allStandards.Where(x => 
                    _cleanser.UnescapeAndRemoveNonAlphanumericCharacters(x.Title.ToLower()).Contains(searchstring.ToLower())).ToList();
        }
    }
}