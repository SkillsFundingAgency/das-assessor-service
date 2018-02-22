using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Application.Api.Middleware;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Exceptions;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [Authorize]
    [Route("api/v1/contacts")]
    public class ContactQueryController : Controller
    {      
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IStringLocalizer<ContactQueryController> _localizer;        
        private readonly ILogger<ContactQueryController> _logger;

        public ContactQueryController(IContactQueryRepository contactQueryRepository,
            IStringLocalizer<ContactQueryController> localizer,       
            ILogger<ContactQueryController> logger
            )
        {           
            _contactQueryRepository = contactQueryRepository;
            _localizer = localizer;           
            _logger = logger;
        }

        [HttpGet("{organisationId}", Name = "GetAllContactsForAnOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactQueryViewModel>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllContactsForAnOrganisation(Guid organisationId)
        {           
            var contacts = await _contactQueryRepository.GetContacts(organisationId);
            return Ok(contacts);
        }

        [HttpGet("user/{userName}/{emailAddress}", Name = "GetContactsByUserNameAndEmail")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactQueryViewModel>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetContactsByUserNameAndEmail(string userName, string emailAddress)
        {
            try
            {
                var contact = await _contactQueryRepository.GetContact(userName, emailAddress);
                return Ok(contact);
            }
            catch (NotFound)
            {
                return NotFound(); 
            }
        }        
    }
}