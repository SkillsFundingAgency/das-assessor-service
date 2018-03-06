namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Maintenance
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net.Http;
    using AssessorService.Api.Types.Models;
    using Dapper;
    using Domain.Consts;  
    using Extensions;
    using FluentAssertions;
    using Newtonsoft.Json;
    using TechTalk.SpecFlow;

    [Binding]
    public sealed class WhenUpdateContact
    {
        private readonly RestClientResult _restClient;
        private readonly IDbConnection _dbconnection;
        private Organisation _organisationQueryViewModel;
        private Contact _contactQueryViewModel;
        private dynamic _contactArgument;

        public WhenUpdateContact(RestClientResult restClient,
          IDbConnection dbconnection)
        {
            _restClient = restClient;
            _dbconnection = dbconnection;
        }

        [When(@"I Update a Contact succesfully")]
        public void WhenIUpdateAContactSuccesfully(IEnumerable<dynamic> contactArguments)
        {
            _contactArgument = contactArguments.First();

            var organisationCreateViewModel = new CreateOrganisationRequest
            {
                EndPointAssessorName = "Test User",
                EndPointAssessorOrganisationId = "9999",
                EndPointAssessorUkprn = 99953456,
                PrimaryContact = null
            };

            CreateOrganisation(organisationCreateViewModel);

            var contactCreateViewModel = new CreateContactRequest
            {
                DisplayName = _contactArgument.UserName + "XXX",
                Email = _contactArgument.Email + "XXX",
                EndPointAssessorOrganisationId = organisationCreateViewModel.EndPointAssessorOrganisationId,
                Username = _contactArgument.UserName
            };

            CreateContact(contactCreateViewModel);

            HttpResponseMessage response = _restClient.HttpClient.GetAsync(
                     $"api/v1/contacts/user/{contactCreateViewModel.DisplayName}").Result;

            _restClient.JsonResult = response.Content.ReadAsStringAsync().Result;
            _restClient.HttpResponseMessage = response;

            _contactQueryViewModel = JsonConvert.DeserializeObject<Contact>(_restClient.JsonResult);

            var contactUpdateViewModel = new UpdateContactRequest
            {
                DisplayName = _contactArgument.DisplayName,
                Email = _contactArgument.Email,
                Username = _contactArgument.UserName
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PutAsJsonAsync(
            "api/v1/contacts", contactUpdateViewModel).Result;
            _restClient.JsonResult = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            _contactQueryViewModel = new Contact
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
             ($"Select Id, UserName, DisplayName, EMail, Status From Contacts where UserName = '{_contactQueryViewModel.Username}'").ToList();
            var contact = contactEntities.First();

            contact.DisplayName.Should().Be(_contactQueryViewModel.DisplayName);
            contact.Email.Should().Be(_contactQueryViewModel.Email);
            contact.Username.Should().Be(_contactQueryViewModel.Username);

            contact.Status.Should().Be(ContactStatus.Live);
        }

        private void CreateOrganisation(CreateOrganisationRequest organisationCreateViewModel)
        {
            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                 "api/v1/organisations", organisationCreateViewModel).Result;
            _restClient.JsonResult = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            _organisationQueryViewModel = JsonConvert.DeserializeObject<Organisation>(_restClient.JsonResult);
        }

        private void CreateContact(CreateContactRequest contactCreateViewModel)
        {
            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
               "api/v1/contacts", contactCreateViewModel).Result;

            _restClient.JsonResult = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;
            _contactQueryViewModel = JsonConvert.DeserializeObject<Contact>(_restClient.JsonResult);
        }
    }
}
