﻿namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger<ContactQueryController> _logger;

        public ContactQueryController(IContactQueryRepository contactQueryRepository,  
            ILogger<ContactQueryController> logger
            )
        {           
            _contactQueryRepository = contactQueryRepository;  
            _logger = logger;
        }

        [HttpGet("{organisationId}", Name = "GetAllContactsForAnOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<Contact>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllContactsForAnOrganisation(Guid organisationId)
        {           
            var contacts = await _contactQueryRepository.GetContacts(organisationId);
            if (contacts.Count() == 0)
                return NotFound();
            return Ok(contacts);
        }

        [HttpGet("user/{userName}", Name = "GetContactsByUserName")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<Contact>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetContactsByUserName(string userName)
        {
            try
            {
                var contact = await _contactQueryRepository.GetContact(userName);
                return Ok(contact);
            }
            catch (NotFound)
            {
                return NotFound(); 
            }
        }        
    }
}