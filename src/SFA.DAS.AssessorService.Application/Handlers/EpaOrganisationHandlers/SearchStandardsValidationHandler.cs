using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class SearchStandardsValidationHandler : IRequestHandler<SearchStandardsValidationRequest, ValidationResponse>
    {
        private readonly IEpaOrganisationValidator _validator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public SearchStandardsValidationHandler(IEpaOrganisationValidator validator, ISpecialCharacterCleanserService cleanser)
        {
            _validator = validator;
            _cleanser = cleanser;
        }

        public async Task<ValidationResponse> Handle(SearchStandardsValidationRequest request, CancellationToken cancellationToken)
        {
            var searchstring = _cleanser.UnescapeAndRemoveNonAlphanumericCharacters(request.Searchstring);

            var result = _validator.ValidatorSearchStandardsRequest(new SearchStandardsValidationRequest
            {
                Searchstring = searchstring
            });

            return await Task.FromResult(result);
        }
    }

}
