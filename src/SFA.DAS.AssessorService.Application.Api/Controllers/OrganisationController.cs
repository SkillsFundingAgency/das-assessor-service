namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using SFA.DAS.AssessorService.Application.Api.Attributes;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    //[Authorize]
    [Route("api/v1/assessment-providers")]
    public class OrganisationController : Controller
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IStringLocalizer<OrganisationController> _localizer;
        private readonly UkPrnValidator _ukPrnValidator;

        public OrganisationController(IOrganisationRepository organisationRepository,
            IStringLocalizer<OrganisationController> localizer,
            UkPrnValidator ukPrnValidator)
        {
            _organisationRepository = organisationRepository;
            _localizer = localizer;
            _ukPrnValidator = ukPrnValidator;
        }

        [HttpGet]
        [HttpGet("{ukprn}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OrganisationQueryViewModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(string))]
        public async Task<IActionResult> Get(int ukprn)
        {
            var result = _ukPrnValidator.Validate(ukprn);
            if (!result.IsValid)
                return BadRequest(result.Errors[0].ErrorMessage);

            var organisation = await _organisationRepository.GetByUkPrn(ukprn);
            if (organisation == null)
            {
                return NotFound(_localizer[ResourceMessageName.NoAssesmentProviderFound, ukprn].Value);
            }

            return Ok(organisation);
        }

        [HttpPost(Name = "Create")]
        [ValidateBadRequest]
        public IActionResult Create(int ukprn,
            [FromBody] OrganisationCreateViewModel organisationCreateViewModel)
        {
            var result = _ukPrnValidator.Validate(ukprn);
            if (!result.IsValid)
                return BadRequest(result.Errors[0].ErrorMessage);

            //if (book == null)
            //{
            //    return BadRequest();
            //}

            //if (book.Description == book.Title)
            //{
            //    ModelState.AddModelError(nameof(BookForCreationDto),
            //        "The provided description should be different from the title.");
            //}

            //if (!ModelState.IsValid)
            //{
            //    // return 422
            //    return new UnprocessableEntityObjectResult(ModelState);
            //}

            //if (!_libraryRepository.AuthorExists(authorId))
            //{
            //    return NotFound();
            //}

            //var bookEntity = Mapper.Map<Book>(book);

            //_libraryRepository.AddBookForAuthor(authorId, bookEntity);

            //if (!_libraryRepository.Save())
            //{
            //    throw new Exception($"Creating a book for author {authorId} failed on save.");
            //}

            //var bookToReturn = Mapper.Map<BookDto>(bookEntity);

            return CreatedAtRoute("Create",
                new { ukprn = 1 },
                new OrganisationQueryViewModel { });
        }
    }
}