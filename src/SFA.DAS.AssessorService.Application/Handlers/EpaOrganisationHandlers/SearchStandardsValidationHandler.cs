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

        public SearchStandardsValidationHandler(IEpaOrganisationValidator validator)
        {
            _validator = validator;
        }

        public async Task<ValidationResponse> Handle(SearchStandardsValidationRequest request, CancellationToken cancellationToken)
        {
            return _validator.ValidatorSearchStandardsRequest(new SearchStandardsValidationRequest
            {
                Searchstring = request.Searchstring
            });
        }
    }

}
