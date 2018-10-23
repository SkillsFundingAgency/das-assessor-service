using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;

namespace SFA.DAS.AssessorService.Api.Types.Models.Register
{ 

    public class SearchStandardsValidationRequest : IRequest<ValidationResponse>
    {
        public string Searchstring { get; set; }
    }

}
