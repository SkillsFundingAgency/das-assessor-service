using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Settings;
using Swashbuckle.AspNetCore.SwaggerGen;
using Contact = SFA.DAS.AssessorService.Domain.Entities.Contact;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/contacts")]
    public class ContactQueryController : Controller
    {
        private readonly SearchOrganisationForContactsValidator _searchOrganisationForContactsValidator;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<ContactQueryController> _logger;
        private readonly IWebConfiguration _config;

        public ContactQueryController(IContactQueryRepository contactQueryRepository,
            SearchOrganisationForContactsValidator searchOrganisationForContactsValidator,
            IMediator mediator,
            ILogger<ContactQueryController> logger, IWebConfiguration config)
        {
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
            _config = config;
            _searchOrganisationForContactsValidator = searchOrganisationForContactsValidator;
            _mediator = mediator;
        }

        [HttpGet("{endPointAssessorOrganisationId}", Name = "SearchContactsForAnOrganisation")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactsForAnOrganisation(string endPointAssessorOrganisationId)
        {
            _logger.LogInformation(
                $"Received Search for Contacts using endPointAssessorOrganisationId = {endPointAssessorOrganisationId}");

            var result = _searchOrganisationForContactsValidator.Validate(endPointAssessorOrganisationId);
            if (!result.IsValid)
                throw new ResourceNotFoundException(result.Errors[0].ErrorMessage);

            var contacts =
                Mapper.Map<List<ContactResponse>>(await _contactQueryRepository.GetContacts(endPointAssessorOrganisationId)).ToList();
            return Ok(contacts);
        }

        [HttpGet("get-all/{endPointAssessorOrganisationId}", Name = "GetAllContactsForOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllContactsForOrganisation(string endPointAssessorOrganisationId)
        {
            _logger.LogInformation(
                $"Received Search for Contacts using endPointAssessorOrganisationId = {endPointAssessorOrganisationId}");

            var result = _searchOrganisationForContactsValidator.Validate(endPointAssessorOrganisationId);
            if (!result.IsValid)
                throw new ResourceNotFoundException(result.Errors[0].ErrorMessage);

            var contacts =
                Mapper.Map<List<ContactResponse>>(await _contactQueryRepository.GetAllContacts(endPointAssessorOrganisationId)).ToList();
            return Ok(contacts);
        }

        [HttpGet("username/{userName}", Name = "SearchContactByUserName")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(ContactResponse))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchContactByUserName(string userName)
        {
            _logger.LogInformation($"Received Search Contact By UserName Request using user name = {userName}");

            var contact = await _contactQueryRepository.GetContact(userName);
            if (contact == null)
                throw new ResourceNotFoundException();
            return Ok(Mapper.Map<ContactResponse>(contact));
        }

        [HttpGet("{endPointAssessorOrganisationId}/withprivileges", Name = "GetAllContactsWithTheirPrivileges")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactResponse>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllContactsWithTheirPrivileges(string endPointAssessorOrganisationId)
        {
            _logger.LogInformation(
                $"Received Search for Contacts and their Privileges using endPointAssessorOrganisationId = {endPointAssessorOrganisationId}");

            return Ok(await _mediator.Send(new GetContactsWithPrivilagesRequest(endPointAssessorOrganisationId)));
        }

        [HttpGet("user/{id}", Name = "GetContactById")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ContactResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetContactById(string id)
        {
            Contact contact = null;
            _logger.LogInformation($" Get Request using user id = {id}");
            try
            {
                var guidId = Guid.Parse(id);
                contact = await _contactQueryRepository.GetContactById(guidId);
            }
            catch (Exception e)
            {
                _logger.LogError(e,$"Failed to retrieve contact with id : {id}");
            }

            if (contact == null) { 
                throw new ResourceNotFoundException();
            }
         
            return Ok(Mapper.Map<ContactResponse>(contact));
        }
        

        [HttpGet("signInId/{signInId}", Name = "GetContactBySignInId")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ContactResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetContactBySignInId(string signInId)
        {
            Contact contact = null;
            _logger.LogInformation($" Get Request using signin id = {signInId}");
            try
            {
                var guidId = Guid.Parse(signInId);
                contact = await _contactQueryRepository.GetBySignInId(guidId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to retrieve contact with signin id : {signInId}");
            }

            if (contact == null)
            {
                throw new ResourceNotFoundException();
            }

            return Ok(Mapper.Map<ContactResponse>(contact));
        }
        
        [HttpPost("MigrateUsers", Name= "MigrateUsers")]
        public async Task<ActionResult> MigrateUsers()
        {
            var endpoint = new Uri(new Uri(_config.DfeSignIn.MetadataAddress), "/Migrate"); 
            using (var httpClient = new HttpClient())
            {
                var usersToMigrate = await _contactQueryRepository.GetUsersToMigrate();
                foreach (var user in usersToMigrate)
                {
                    var result = await httpClient.PostAsJsonAsync(endpoint, new
                    {
                        ClientId = _config.DfeSignIn.ClientId, 
                        GivenName = user.GivenNames, 
                        FamilyName = user.FamilyName, 
                        Email = user.Email
                    });

                    var migrateResult = await result.Content.ReadAsAsync<MigrateUserResult>();

                    await _contactQueryRepository.UpdateMigratedContact(user.Id, migrateResult.NewUserId);
                }
            }
            
            
            return Ok(); 
        }

        [HttpPost("MigrateContactsAndOrgsToApply", Name = "MigrateContactsAndOrgsToApply")]
        public async Task<ActionResult> MigrateContactsAndOrgsToApply()
        {
            var endpoint = new Uri(new Uri(_config.ApplyBaseAddress), "/MigrateContactAndOrgs");
            using (var httpClient = new HttpClient())
            {
                var contactsToMigrate = await _contactQueryRepository.GetExsitingContactsToMigrateToApply();
                foreach (var contact in contactsToMigrate)
                {
                    var request = MapAssessorToApply(contact);
                    
                    await httpClient.PostAsJsonAsync(endpoint, request);

                }
            }
            return Ok();
        }

        [HttpPost("MigrateSingleContactToApply", Name = "MigrateSingleContactToApply")]
        public async Task<ActionResult> MigrateSingleContactToApply([FromBody]SigninIdWrapper signinWrapper)
        {
            var endpoint = new Uri(new Uri(_config.ApplyBaseAddress), "/MigrateContactAndOrgs");
            using (var httpClient = new HttpClient())
            {
                var contactToMigrate = await _contactQueryRepository.GetSingleContactsToMigrateToApply(signinWrapper.SigninId);
                var request = MapAssessorToApply(contactToMigrate);
                await httpClient.PostAsJsonAsync(endpoint, request);
            }

            return Ok();
        }

        private MigrateContactOrganisation MapAssessorToApply(Contact contact)
        {
            var request = new MigrateContactOrganisation
            {
                contact = new ApplyTypes.Contact
                {
                    Id = contact.Id,
                    CreatedAt = contact.CreatedAt,
                    CreatedBy = contact.Id.ToString(),
                    DeletedAt = null,
                    DeletedBy = null,
                    Email = contact.Email,
                    FamilyName = contact.FamilyName,
                    GivenNames = contact.GivenNames,
                    IsApproved = true,
                    SigninId = contact.SignInId,
                    SigninType = "ASLogin",
                    ApplyOrganisationId = Guid.Empty,
                    Status = "Live",
                    UpdatedAt = null,
                    UpdatedBy = null
                }
            };


            if (contact.Organisation != null)
            {
                request.organisation = new ApplyTypes.Organisation
                {
                    CreatedAt = contact.Organisation.CreatedAt,
                    CreatedBy = contact.Id.ToString(),
                    DeletedAt = null,
                    DeletedBy = null,
                    Id = Guid.Empty,
                    Name = contact.Organisation.EndPointAssessorName,
                    OrganisationType = contact.Organisation.OrganisationType?.Type,
                    OrganisationUkprn = contact.Organisation.EndPointAssessorUkprn,
                    RoATPApproved = true,
                    RoEPAOApproved = true,
                    Status = "New",
                    UpdatedAt = null,
                    UpdatedBy = null,
                    OrganisationDetails = new OrganisationDetails
                    {
                        Address1 = contact.Organisation.OrganisationDataFromJson?.Address1,
                        Address2 = contact.Organisation.OrganisationDataFromJson?.Address2,
                        Address3 = contact.Organisation.OrganisationDataFromJson?.Address3,
                        CharityNumber = contact.Organisation.OrganisationDataFromJson?.CharityNumber,
                        City = contact.Organisation.OrganisationDataFromJson?.Address4,
                        CompanyNumber = contact.Organisation.OrganisationDataFromJson?.CompanyNumber,
                        LegalName = contact.Organisation.OrganisationDataFromJson?.LegalName,
                        OrganisationReferenceId = contact.Organisation.EndPointAssessorUkprn == null
                            ? contact.Organisation.EndPointAssessorOrganisationId
                            : contact.Organisation.EndPointAssessorUkprn.ToString(),
                        OrganisationReferenceType = "RoEPAO",
                        Postcode = contact.Organisation.OrganisationDataFromJson?.Postcode,
                        ProviderName = null,
                        TradingName = contact.Organisation.OrganisationDataFromJson?.TradingName,
                        FHADetails = new FHADetails
                        {
                            FinancialDueDate = contact.Organisation.OrganisationDataFromJson?.FhaDetails?.FinancialDueDate,
                            FinancialExempt = contact.Organisation.OrganisationDataFromJson?.FhaDetails?.FinancialExempt
                        },
                        EndPointAssessmentOrgId = contact.EndPointAssessorOrganisationId
                    }
                };
            }

            return request;
        }
    }
    public class MigrateUserResult
    {
        public Guid NewUserId { get; set; }
    }

    public class MigrateContactOrganisation
    {
        public ApplyTypes.Contact contact { get; set; }
        public Organisation organisation { get; set; }
    }
}