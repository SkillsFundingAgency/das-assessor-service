﻿namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using FluentAssertions;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using TechTalk.SpecFlow;
    using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
    using System.Data;
    using Dapper;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using SFA.DAS.AssessorService.Domain.Enums;
    using System.Net.Http;

    [Binding]
    public sealed class WhenUpdateContact
    {
        private RestClient _restClient;
        private readonly IDbConnection _dbconnection;
        private Organisation _organisationQueryViewModel;
        private Contactl _contactQueryViewModel;
        private dynamic _contactArgument;

        public WhenUpdateContact(RestClient restClient,
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
                EndPointAssessorUKPRN = 99953456,
                PrimaryContactId = null
            };

            CreateOrganisation(organisationCreateViewModel);

            var contactCreateViewModel = new CreateContactRequest
            {
                ContactName = _contactArgument.ContactName + "XXX",
                ContactEmail = _contactArgument.ContactEmail + "XXX",
                EndPointAssessorContactId = 99953456,
                OrganisationId = _organisationQueryViewModel.Id
            };

            CreateContact(contactCreateViewModel);

            HttpResponseMessage response = _restClient.HttpClient.GetAsync(
                     $"api/v1/contacts/user/{contactCreateViewModel.ContactName}/{contactCreateViewModel.ContactEmail}").Result;

            _restClient.Result = response.Content.ReadAsStringAsync().Result;
            _restClient.HttpResponseMessage = response;

            _contactQueryViewModel = JsonConvert.DeserializeObject<Contactl>(_restClient.Result);

            var contactUpdateViewModel = new UpdateContactRequest
            {
                ContactName = _contactArgument.ContactName,
                ContactEmail = _contactArgument.ContactEmail,
                Id = _contactQueryViewModel.Id
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PutAsJsonAsync(
            "api/v1/contacts", contactUpdateViewModel).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            _contactQueryViewModel = new Contactl
            {
                ContactName = _contactArgument.ContactName,
                ContactEmail = _contactArgument.ContactEmail,
                Id = _contactQueryViewModel.Id
            };
        }

        [Then(@"the Contact Update should have occured")]
        public void ThenTheContactUpdateShouldHaveOccured()
        {
            var contactEntities = _dbconnection.Query<Contactl>
             ($"Select Id, ContactName, ContactEMail, ContactStatus From Contacts where Id = '{_contactQueryViewModel.Id}'").ToList();
            var contact = contactEntities.First();

            contact.ContactName.Should().Be(_contactQueryViewModel.ContactName);
            contact.ContactEmail.Should().Be(_contactQueryViewModel.ContactEmail);

            contact.ContactStatus.Should().Be(ContactStatus.Live);
        }

        private void CreateOrganisation(CreateOrganisationRequest organisationCreateViewModel)
        {
            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                 "api/v1/organisations", organisationCreateViewModel).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            _organisationQueryViewModel = JsonConvert.DeserializeObject<Organisation>(_restClient.Result);
        }

        private void CreateContact(CreateContactRequest contactCreateViewModel)
        {
            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
               "api/v1/contacts", contactCreateViewModel).Result;

            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;
            _contactQueryViewModel = JsonConvert.DeserializeObject<Contactl>(_restClient.Result);
        }
    }
}
