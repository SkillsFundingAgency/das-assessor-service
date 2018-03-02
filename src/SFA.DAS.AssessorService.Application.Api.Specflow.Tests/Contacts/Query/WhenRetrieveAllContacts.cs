﻿namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using SFA.DAS.AssessorService.Api.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using AssessorService.Api.Types.Models;
    using TechTalk.SpecFlow;

    [Binding]
    public class WhenRetrieveAllContacts
    {
        private readonly RestClient _restClient;
        private List<Organisation> _organisationQueryViewModels = new List<Organisation>();
        private List<Contact> _contactQueryViewModels = new List<Contact>();

        public WhenRetrieveAllContacts(RestClient restClient)
        {
            _restClient = restClient;
        }


        [When(@"I Request All Contacts to be retrieved BY Organisation")]
        public void WhenIRequestAllContactsToBeRetrievedBYOrganisation()
        {
            HttpResponseMessage response = _restClient.HttpClient.GetAsync(
               "api/v1/organisations").Result;
            if (response.IsSuccessStatusCode)
            {
                _restClient.Result = response.Content.ReadAsStringAsync().Result;
                _restClient.HttpResponseMessage = response;

                _organisationQueryViewModels = JsonConvert.DeserializeObject<List<Organisation>>(_restClient.Result);

                var organisation = _organisationQueryViewModels.First(q => q.EndPointAssessorUkprn == 10000000);

                response = _restClient.HttpClient.GetAsync(
                        $"api/v1/contacts/{organisation.EndPointAssessorOrganisationId}").Result;


                _contactQueryViewModels = JsonConvert.DeserializeObject<List<Contact>>(_restClient.Result);

                _restClient.HttpResponseMessage = response;
            }
            else
            {
                throw new ApplicationException("Cannot find any organisations in database");
            }
        }

        [When(@"I Request All Contacts to be retrieved By an Invalid Organisation")]
        public void WhenIRequestAllContactsToBeRetrievedByAnInvalidOrganisation()
        {
            var id = Guid.NewGuid();

            HttpResponseMessage response = _restClient.HttpClient.GetAsync(
                        $"api/v1/contacts/{id}").Result;

            _restClient.HttpResponseMessage = response;
        }

        [Then(@"the API returns all Contacts for an Organisation")]
        public void ThenTheAPIReturnsAllContactsForAnOrganisation()
        {
            _contactQueryViewModels.Count.Should().BeGreaterOrEqualTo(1);
        }
    }
}
