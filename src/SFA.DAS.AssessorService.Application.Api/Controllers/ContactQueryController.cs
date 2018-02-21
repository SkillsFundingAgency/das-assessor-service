namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Exceptions;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    //[Authorize]
    [Route("api/v1/contacts")]
    public class ContactQueryController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IContactRepository _contactRepository;
        private readonly IStringLocalizer<ContactQueryController> _localizer;        
        private readonly ILogger<ContactQueryController> _logger;

        public ContactQueryController(IMediator mediator,
            IContactRepository contactRepository,
            IStringLocalizer<ContactQueryController> localizer,       
            ILogger<ContactQueryController> logger
            )
        {
            _mediator = mediator;
            _contactRepository = contactRepository;
            _localizer = localizer;           
            _logger = logger;
        }

        [HttpGet("{organisationId}", Name = "GetAllContactsForAnOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OrganisationQueryViewModel))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(string))]
        public async Task<IActionResult> GetAllContactsForAnOrganisation(Guid organisationId)
        {           
            var contacts = await _contactRepository.GetContacts(organisationId);
            return Ok(contacts);
        }

        [HttpGet("user/{userName}/{emailAddress}", Name = "GetContactsByUserNameAndEmail")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(OrganisationQueryViewModel))]
        public async Task<IActionResult> GetContactsByUserNameAndEmail(string userName, string emailAddress)
        {
            try
            {
                var contact = await _contactRepository.GetContact(userName, emailAddress);
                return Ok(contact);
            }
            catch (NotFound)
            {
                return NotFound(); 
            }
        }        
    }
}