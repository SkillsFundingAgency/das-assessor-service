using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Api.Validators.ExternalApi.Learners;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.ExternalApi
{
    [Authorize]
    [ValidateBadRequest]
    [Route("api/v1/learners/batch/")]
    public class LearnerBatchController : Controller
    {
        private readonly IMediator _mediator;
        private readonly GetBatchLearnerRequestValidator _getValidator;

        public LearnerBatchController(IMediator mediator, GetBatchLearnerRequestValidator getValidator)
        {
            _mediator = mediator;
            _getValidator = getValidator;
        }

        [HttpGet("{uln}/{lastname}/{standard}/{ukPrn}/{*email}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GetBatchLearnerResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Get(long uln, string lastname, string standard, int ukPrn, string email)
        {
            var request = new GetBatchLearnerRequest
            {
                Uln = uln,
                FamilyName = lastname,
                Standard = standard,
                IncludeCertificate = true,
                UkPrn = ukPrn,
                Email = email
            };

            var validationResult = await _getValidator.ValidateAsync(request);
            var isRequestValid = validationResult.IsValid;
            var validationErrors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

            var getResponse = new GetBatchLearnerResponse
            {
                ValidationErrors = validationErrors
            };

            if (!validationErrors.Any() && isRequestValid)
            {
                var result = await _mediator.Send(request);

                getResponse.Learner = result.Learner;
                getResponse.Certificate = result.Certificate;
            }

            return Ok(getResponse);
        }
    }
}