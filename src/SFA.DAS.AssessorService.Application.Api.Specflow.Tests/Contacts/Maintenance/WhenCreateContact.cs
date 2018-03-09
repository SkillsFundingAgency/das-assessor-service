using System.Collections.Generic;
using System.Data;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Maintenance.Services;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance.Services;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Maintenance
{
    [Binding]
    public sealed class WhenCreateContact
    {
        private readonly ContactService _contactService;
        private readonly CreateContactBuilder _createContactBuilder;
        private readonly CreateOrganisationBuilder _createOrganisationBuilder;
        private readonly IDbConnection _dbconnection;
        private readonly OrganisationService _organisationService;
        private readonly OrganisationQueryService _organisationQueryService;
        private dynamic _contactArgument;
        private ContactResponse _contactResponse;
        private CreateOrganisationRequest _createOrganisationRequest;

        private OrganisationResponse _organisaionRetrieved;

        private RestClientResult _restClient;

        public WhenCreateContact(RestClientResult restClient,
            OrganisationService organisationService,
            OrganisationQueryService organisationQueryService,
            ContactService contactService,
            CreateOrganisationBuilder createOrganisationBuilder,
            CreateContactBuilder createContactBuilder,
            IDbConnection dbconnection)
        {
            _restClient = restClient;
            _organisationService = organisationService;
            _organisationQueryService = organisationQueryService;
            _contactService = contactService;
            _createOrganisationBuilder = createOrganisationBuilder;
            _createContactBuilder = createContactBuilder;
            _dbconnection = dbconnection;
        }

        [When(@"I Create a Contact as First User for Organisation")]
        public void WhenICreateAContactAsFirstUserForOrganisation(IEnumerable<dynamic> contactArguments)
        {
            _contactArgument = contactArguments.First();

            _createOrganisationRequest = new CreateOrganisationRequest
            {
                EndPointAssessorName = "Test User",
                EndPointAssessorOrganisationId = "994432",
                EndPointAssessorUkprn = 99953456,
                PrimaryContact = null
            };

            _restClient = _organisationService.PostOrganisation(_createOrganisationRequest);

            var contact = _createContactBuilder.Build(_contactArgument, _createOrganisationRequest.EndPointAssessorOrganisationId);

            _restClient = _contactService.PostContact(contact);

            _contactResponse = _restClient.Deserialise<ContactResponse>();
        }

        [When(@"I Create a Contact as another User for Organisation")]
        public void WhenICreateAContactAsAnotherUserForOrganisation(IEnumerable<dynamic> contactArguments)
        {
            _contactArgument = contactArguments.First();

            _createOrganisationRequest = new CreateOrganisationRequest
            {
                EndPointAssessorName = "Test User",
                EndPointAssessorOrganisationId = "99944",
                EndPointAssessorUkprn = 99953456,
                PrimaryContact = null
            };

            _restClient = _organisationService.PostOrganisation(_createOrganisationRequest);

            var contactRequest = new CreateContactRequest
            {
                DisplayName = _contactArgument.DisplayName + "XXX",
                Email = _contactArgument.Email + "XXX",
                EndPointAssessorOrganisationId = _createOrganisationRequest.EndPointAssessorOrganisationId,
                UserName = "DummyUser123"
            };

            _restClient = _contactService.PostContact(contactRequest);

            contactRequest = _createContactBuilder.Build(_contactArgument, _createOrganisationRequest.EndPointAssessorOrganisationId);
            _restClient = _contactService.PostContact(contactRequest);

            _contactResponse = _restClient.Deserialise<ContactResponse>();
        }


        [When(@"I Create a Contact That already exists")]
        public void WhenICreateAContactThatAlreadyExists(IEnumerable<dynamic> contactArguments)
        {
            _contactArgument = contactArguments.First();

            _createOrganisationRequest = new CreateOrganisationRequest
            {
                EndPointAssessorName = "Test User",
                EndPointAssessorOrganisationId = "994433",
                EndPointAssessorUkprn = 99953456,
                PrimaryContact = null
            };

            _restClient = _organisationService.PostOrganisation(_createOrganisationRequest);

            var contactRequest = _createContactBuilder.Build(_contactArgument, _createOrganisationRequest.EndPointAssessorOrganisationId);
            _restClient = _contactService.PostContact(contactRequest);

            contactRequest = _createContactBuilder.Build(_contactArgument, _createOrganisationRequest.EndPointAssessorOrganisationId);
            _restClient = _contactService.PostContact(contactRequest);
            _contactResponse = _restClient.Deserialise<ContactResponse>();

        }

        [Then(@"the Contact should be created")]
        public void ThenTheContactShouldBeCreated()
        {
            _contactResponse.DisplayName.Should().Be(_contactArgument.DisplayName);
            _contactResponse.Email.Should().Be(_contactArgument.Email);
        }

        [Then(@"the Contact Status should be set to Live")]
        public void ThenTheContactStatusShouldBeSetToLive()
        {
            _contactResponse.Status.Should().Be(ContactStatus.Live);
        }

        [Then(@"the Contact Organisation Status should be set to (.*)")]
        public void ThenTheContactOrganisationStatusShouldBeSetTo(string p0)
        {
            RetrieveOrganisation(_createOrganisationRequest);

            _organisaionRetrieved.Status.Should().Be(OrganisationStatus.Live);
        }

        private void RetrieveOrganisation(CreateOrganisationRequest createOrganisationRequest)
        {
            var result =
                _organisationQueryService.SearchOrganisationByUkPrn(createOrganisationRequest.EndPointAssessorUkprn);
            _organisaionRetrieved = result.Deserialise<OrganisationResponse>();
        }

        private void CreateContact(CreateContactRequest createContactRequest)
        {
            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                "api/v1/contacts", createContactRequest).Result;

            _restClient.JsonResult = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;
            _contactResponse = JsonConvert.DeserializeObject<ContactResponse>(_restClient.JsonResult);
        }
    }
}