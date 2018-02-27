namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using SFA.DAS.AssessorService.Api.Types;
    using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
    using SFA.DAS.AssessorService.Domain.Enums;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
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
                EndPointAssessorUKPRN = 99953456,
                PrimaryContactId = null
            };

            CreateOrganisation(organisationCreateViewModel);

            var contactCreateViewModel = new CreateContactRequest
            {
                ContactName = _contactArguments.ContactName,
                ContactEmail = _contactArguments.ContactEmail,
                EndPointAssessorContactId = 99953456,
                OrganisationId = _organisationQueryViewModel.Id
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
                EndPointAssessorUKPRN = 99953456,
                PrimaryContactId = null
            };

            CreateOrganisation(organisationCreateViewModel);

            var contactCreateViewModel = new CreateContactRequest
            {
                ContactName = _contactArguments.ContactName + "XXX",
                ContactEmail = _contactArguments.ContactEmail + "XXX",
                EndPointAssessorContactId = 99953457,
                OrganisationId = _organisationQueryViewModel.Id
            };

            CreateContact(contactCreateViewModel);

            contactCreateViewModel = new CreateContactRequest
            {
                ContactName = _contactArguments.ContactName,
                ContactEmail = _contactArguments.ContactEmail,
                EndPointAssessorContactId = 99953456,
                OrganisationId = _organisationQueryViewModel.Id
            };
            CreateContact(contactCreateViewModel);

            RetrieveOrganisation(organisationCreateViewModel);
        }

        [Then(@"the Contact should be created")]
        public void ThenTheContactShouldBeCreated()
        {
            _contactQueryViewModel.ContactName.Should().Be(_contactArguments.ContactName);
            _contactQueryViewModel.ContactEmail.Should().Be(_contactArguments.ContactEmail);
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
              $"api/v1/organisations/{organisationCreateViewModel.EndPointAssessorUKPRN}").Result;
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
                EndPointAssessorUKPRN = 99953456,
                PrimaryContactId = null
            };

            CreateOrganisation(organisationCreateViewModel);

            var contactCreateViewModel = new CreateContactRequest
            {
                ContactName = _contactArguments.ContactName,
                ContactEmail = _contactArguments.ContactEmail,
                EndPointAssessorContactId = 99953456,
                OrganisationId = _organisationQueryViewModel.Id
            };

            CreateContact(contactCreateViewModel);

            contactCreateViewModel = new CreateContactRequest
            {
                ContactName = _contactArguments.ContactName,
                ContactEmail = _contactArguments.ContactEmail,
                EndPointAssessorContactId = 99953456,
                OrganisationId = _organisationQueryViewModel.Id
            };
            CreateContact(contactCreateViewModel);
        }
    }
}
