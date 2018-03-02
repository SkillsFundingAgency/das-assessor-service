namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using AssessorService.Api.Types.Models;
    using Domain.Enums;
    using Extensions;
    using FluentAssertions;
    using Newtonsoft.Json;
    using TechTalk.SpecFlow;

    [Binding]
    public sealed class WhenCreateContact
    {
        private RestClient _restClient;
        private readonly IDbConnection _dbconnection;
        private Organisation _organisationQueryViewModel,
            _organisaionRetrieved;
        private Contact _contactQueryViewModel;
        private dynamic _contactArguments;

        public WhenCreateContact(RestClient restClient,
          IDbConnection dbconnection)
        {
            _restClient = restClient;
            _dbconnection = dbconnection;
        }

        [When(@"I Create a Contact as First User for Organisation")]
        public void WhenICreateAContactAsFirstUserForOrganisation(IEnumerable<dynamic> contactArguments)
        {
            _contactArguments = contactArguments.First();

            var organisationCreateViewModel = new CreateOrganisationRequest
            {
                EndPointAssessorName = "Test User",
                EndPointAssessorOrganisationId = "9999999994432",
                EndPointAssessorUkprn = 99953456,
                PrimaryContact = null
            };

            CreateOrganisation(organisationCreateViewModel);

            var contactCreateViewModel = new CreateContactRequest
            {
                Username = _contactArguments.UserName,
                DisplayName = _contactArguments.DisplayName,
                Email = _contactArguments.Email,
                EndPointAssessorOrganisationId = organisationCreateViewModel.EndPointAssessorOrganisationId
            };

            CreateContact(contactCreateViewModel);

            RetrieveOrganisation(organisationCreateViewModel);
        }

        private void CreateContact(CreateContactRequest contactCreateViewModel)
        {
            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
               "api/v1/contacts", contactCreateViewModel).Result;

            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;
            _contactQueryViewModel = JsonConvert.DeserializeObject<Contact>(_restClient.Result);
        }

        private void CreateOrganisation(CreateOrganisationRequest organisationCreateViewModel)
        {
            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                 "api/v1/organisations", organisationCreateViewModel).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            _organisationQueryViewModel = JsonConvert.DeserializeObject<Organisation>(_restClient.Result);
        }

        [When(@"I Create a Contact as another User for Organisation")]
        public void WhenICreateAContactAsAnotherUserForOrganisation(IEnumerable<dynamic> contactArguments)
        {
            _contactArguments = contactArguments.First();

            var organisationCreateViewModel = new CreateOrganisationRequest
            {
                EndPointAssessorName = "Test User",
                EndPointAssessorOrganisationId = "99944",
                EndPointAssessorUkprn = 99953456,
                PrimaryContact = null
            };

            CreateOrganisation(organisationCreateViewModel);

            var contactCreateViewModel = new CreateContactRequest
            {
                DisplayName = _contactArguments.DisplayName + "XXX",
                Email = _contactArguments.Email + "XXX",
                EndPointAssessorOrganisationId = organisationCreateViewModel.EndPointAssessorOrganisationId,
                Username = "DummyUser123"
            };

            CreateContact(contactCreateViewModel);

            contactCreateViewModel = new CreateContactRequest
            {
                DisplayName = _contactArguments.DisplayName,
                Email = _contactArguments.Email,
                EndPointAssessorOrganisationId = organisationCreateViewModel.EndPointAssessorOrganisationId,
                Username = _contactArguments.UserName
            };
            CreateContact(contactCreateViewModel);

            RetrieveOrganisation(organisationCreateViewModel);
        }

        [Then(@"the Contact should be created")]
        public void ThenTheContactShouldBeCreated()
        {
            _contactQueryViewModel.DisplayName.Should().Be(_contactArguments.DisplayName);
            _contactQueryViewModel.Email.Should().Be(_contactArguments.Email);
        }

        [Then(@"the Contact Status should be set to Live")]
        public void ThenTheContactStatusShouldBeSetToLive()
        {
            _contactQueryViewModel.ContactStatus.Should().Be(ContactStatus.Live);
        }

        [Then(@"the Contact Organisation Status should be set to (.*)")]
        public void ThenTheContactOrganisationStatusShouldBeSetTo(string p0)
        {
            _organisaionRetrieved.OrganisationStatus.Should().Be(OrganisationStatus.Live);
        }

        private void RetrieveOrganisation(CreateOrganisationRequest organisationCreateViewModel)
        {
            var organisationResponseMessage = _restClient.HttpClient.GetAsync(
              $"api/v1/organisations/{organisationCreateViewModel.EndPointAssessorUkprn}").Result;
            var result = organisationResponseMessage.Content.ReadAsStringAsync().Result;
            _organisaionRetrieved = JsonConvert.DeserializeObject<Organisation>(result);
        }


        [When(@"I Create a Contact That already exists")]
        public void WhenICreateAContactThatAlreadyExists(IEnumerable<dynamic> contactArguments)
        {
            _contactArguments = contactArguments.First();

            var organisationCreateViewModel = new CreateOrganisationRequest
            {
                EndPointAssessorName = "Test User",
                EndPointAssessorOrganisationId = "9999999994433",
                EndPointAssessorUkprn = 99953456,
                PrimaryContact = null
            };

            CreateOrganisation(organisationCreateViewModel);

            var contactCreateViewModel = new CreateContactRequest
            {
                DisplayName = _contactArguments.DisplayName,
                Email = _contactArguments.Email,
                EndPointAssessorOrganisationId = organisationCreateViewModel.EndPointAssessorOrganisationId,
                Username = _contactArguments.UserName
            };

            CreateContact(contactCreateViewModel);

            contactCreateViewModel = new CreateContactRequest
            {
                DisplayName = _contactArguments.DisplayName,
                Email = _contactArguments.Email,
                EndPointAssessorOrganisationId = organisationCreateViewModel.EndPointAssessorOrganisationId,
                Username = _contactArguments.UserName
            };
            CreateContact(contactCreateViewModel);
        }
    }
}
