using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Maintenance.Services;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Maintenance
{
    [Binding]
    public sealed class WhenDeleteContact
    {       
        private readonly ContactService _contactService;
        private readonly CreateContactBuilder _createContactBuilder;
        private readonly IDbConnection _dbconnection;        
        private readonly OrganisationService _organisationService;
        private dynamic _contactArgument;     

        private List<OrganisationResponse> _organisations = new List<OrganisationResponse>();
        private RestClientResult _restClient;

        public WhenDeleteContact(RestClientResult restClient,          
            OrganisationService organisationService,
            ContactService contactService,            
            CreateContactBuilder createContactBuilder,
            IDbConnection dbconnection)
        {
            _restClient = restClient;         
            _organisationService = organisationService;
            _contactService = contactService;            
            _createContactBuilder = createContactBuilder;
            _dbconnection = dbconnection;
        }

        [When(@"I Delete a Contact")]
        public void WhenIDeleteAContact(IEnumerable<dynamic> contactArguments)
        {
            _contactArgument = contactArguments.First();

            CreateOrganisation();
            CreateContactRequest createContactRequest = CreateContact();

            _contactService.DeleteContact(createContactRequest.UserName);
        }

        [When(@"I Delete a Contact Twice")]
        public void WhenIDeleteAContactTwice(IEnumerable<
            dynamic> contactArguments)
        {
            _contactArgument = contactArguments.First();

            CreateOrganisation();
            CreateContactRequest createContactRequest = CreateContact();

            _contactService.DeleteContact(createContactRequest.UserName);
            _contactService.DeleteContact(createContactRequest.UserName);
        }

        [Then(@"the Contact should be deleted")]
        public void ThenTheContactShouldBeDeleted()
        {
            var contacts = _dbconnection.Query<ContactResponse>
                ($"Select Status From Contacts where UserName = '{_contactArgument.UserName}'").ToList();
            var contact = contacts.First();

            contact.Status.Should().Be(OrganisationStatus.Deleted);
        }

        private void CreateOrganisation()
        {
            var createOrganisationRequest = new CreateOrganisationRequest
            {
                EndPointAssessorName = "Test User",
                EndPointAssessorOrganisationId = _contactArgument.EndPointAssessorOrganisationId.ToString(),
                EndPointAssessorUkprn = 99953456,
                PrimaryContact = null
            };

            _restClient = _organisationService.PostOrganisation(createOrganisationRequest);
        }


        private CreateContactRequest CreateContact()
        {
            var createContactRequest = _createContactBuilder.Build(_contactArgument,
                _contactArgument.EndPointAssessorOrganisationId.ToString());
            _contactService.PostContact(createContactRequest);
            return createContactRequest;
        }

    }
}