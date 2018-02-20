namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Application.Api.Attributes;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Exceptions;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    //[Authorize]
    [Route("api/v1/assessment-users")]
    public class ContactController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IStringLocalizer<OrganisationController> _localizer;
        private readonly UkPrnValidator _ukPrnValidator;
        private readonly ILogger<OrganisationController> _logger;

        public ContactController(IMediator mediator,         
            IStringLocalizer<OrganisationController> localizer,
            UkPrnValidator ukPrnValidator,
            ILogger<OrganisationController> logger
            )
        {
            _mediator = mediator;           
            _localizer = localizer;
            _ukPrnValidator = ukPrnValidator;
            _logger = logger;
        }

        //[HttpGet("{ukprn}")]
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ContactQueryViewModel))]
        //[SwaggerResponse((int)HttpStatusCode.BadRequest)]
        //[SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(string))]
        //public async Task<IActionResult> Get(int ukprn)
        //{
        //    var result = _ukPrnValidator.Validate(ukprn);
        //    if (!result.IsValid)
        //        return BadRequest(result.Errors[0].ErrorMessage);

        //    var organisation = await _organisationRepository.GetByUkPrn(ukprn);
        //    if (organisation == null)
        //    {
        //        return NotFound(_localizer[ResourceMessageName.NoAssesmentProviderFound, ukprn].Value);
        //    }

        //    return Ok(organisation);
        //}

        //[HttpGet]
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ContactQueryViewModel))]
        //public async Task<IActionResult> Get()
        //{
        //    var organisations = await _organisationRepository.GetAllOrganisations();
        //    return Ok(organisations);
        //}

        [HttpPost(Name = "CreateContract")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(ContactQueryViewModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ContactQueryViewModel))]
        public async Task<IActionResult> CreateContract(
            [FromBody] ContactCreateViewModel contactCreateViewModel)
        {
            _logger.LogInformation("Received Create Contact Request");
        
            var contactQueryViewModel = await _mediator.Send(contactCreateViewModel);

            return CreatedAtRoute("CreateContract",
                new { Id = contactQueryViewModel.Id },
                contactQueryViewModel);
        }

        //[HttpPut(Name = "Update")]
        //[ValidateBadRequest]
        //[SwaggerResponse((int)HttpStatusCode.NoContent, Type = typeof(ContactQueryViewModel))]
        //[SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ContactQueryViewModel))]
        //public async Task<IActionResult> Update(int ukprn,
        //  [FromBody] OrganisationUpdateViewModel organisationUpdateViewModel)
        //{
        //    _logger.LogInformation("Received Update Request");

        //    var result = _ukPrnValidator.Validate(ukprn);
        //    if (!result.IsValid)
        //        return BadRequest(result.Errors[0].ErrorMessage);

        //    var ContactQueryViewModel = await _mediator.Send(organisationUpdateViewModel);

        //    return NoContent();
        //}

        //[HttpDelete(Name = "Delete")]
        //[ValidateBadRequest]
        //[SwaggerResponse((int)HttpStatusCode.NoContent)]
        //[SwaggerResponse((int)HttpStatusCode.BadRequest)]
        //[SwaggerResponse((int)HttpStatusCode.NotFound)]
        //public async Task<IActionResult> Delete(int ukprn)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Received Update Request");

        //        var result = _ukPrnValidator.Validate(ukprn);
        //        if (!result.IsValid)
        //            return BadRequest(result.Errors[0].ErrorMessage);

        //        var organisationDeleteViewModel = new OrganisationDeleteViewModel
        //        {
        //            UKPrn = ukprn
        //        };

        //        await _mediator.Send(organisationDeleteViewModel);

        //        return NoContent();
        //    }
        //    catch (NotFound exception)
        //    {             
        //        return NotFound();
        //    }
        //}
    }
}