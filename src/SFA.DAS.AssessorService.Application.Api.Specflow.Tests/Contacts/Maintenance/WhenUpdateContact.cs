using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Maintenance.Services;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Maintenance
{
    [Binding]
    public sealed class WhenUpdateContact
    {
        private readonly ContactQueryService _contactQueryService;
        private readonly ContactService _contactService;
        private readonly CreateOrganisationBuilder _createOrganisationBuilder;
        private readonly IDbConnection _dbconnection;
        private readonly OrganisationService _organisationService;
        private dynamic _contactArgument;
        private Contact _contactArguments;
        private RestClientResult _restClient;

        public WhenUpdateContact(RestClientResult restClient,
            OrganisationService organisationService,
            CreateOrganisationBuilder createOrganisationBuilder,
            ContactService contactService,
            ContactQueryService contactQueryService,
            IDbConnection dbconnection)
        {
            _restClient = restClient;
            _organisationService = organisationService;
            _createOrganisationBuilder = createOrganisationBuilder;
            _contactService = contactService;
            _contactQueryService = contactQueryService;
            _dbconnection = dbconnection;
        }

        [When(@"I Update a Contact succesfully")]
        public void WhenIUpdateAContactSuccesfully(IEnumerable<dynamic> contactArguments)
        {
            _contactArgument = contactArguments.First();

            var createOrganisationRequest = new CreateOrganisationRequest
            {
                EndPointAssessorName = "Test User",
                EndPointAssessorOrganisationId = "9999",
                EndPointAssessorUkprn = 99953456,
                PrimaryContact = null
            };
            _organisationService.PostOrganisation(createOrganisationRequest);

            var contactRequest = new CreateContactRequest
            {
                DisplayName = _contactArgument.UserName + "XXX",
                Email = _contactArgument.Email + "XXX",
                EndPointAssessorOrganisationId = createOrganisationRequest.EndPointAssessorOrganisationId,
                Username = _contactArgument.UserName
            };

            _contactService.PostContact(contactRequest);

            _restClient = _contactQueryService.SearchForContactByUserName(contactRequest.Username);

            var updateContactRequest = new UpdateContactRequest
            {
                DisplayName = _contactArgument.DisplayName,
                Email = _contactArgument.Email,
                Username = _contactArgument.UserName
            };

            _contactService.PutContact(updateContactRequest);

            _contactArguments = new Contact
            {
                DisplayName = _contactArgument.DisplayName,
                Email = _contactArgument.Email,
                Username = _contactArgument.UserName
            };
        }

        [Then(@"the Contact Update should have occured")]
        public void ThenTheContactUpdateShouldHaveOccured()
        {
            var contactEntities = _dbconnection.Query<Contact>
                    ($"Select Id, UserName, DisplayName, EMail, Status From Contacts where UserName = '{_contactArguments.Username}'")
                .ToList();
            var contact = contactEntities.First();

            contact.DisplayName.Should().Be(_contactArguments.DisplayName);
            contact.Email.Should().Be(_contactArguments.Email);
            contact.Username.Should().Be(_contactArguments.Username);

            contact.Status.Should().Be(ContactStatus.Live);
        }
    }
}