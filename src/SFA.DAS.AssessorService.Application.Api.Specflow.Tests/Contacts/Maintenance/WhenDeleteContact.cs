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
    public sealed class WhenDeleteContact
    {
        private RestClient _restClient;
        private readonly IDbConnection _dbconnection;
        private Organisation _organisationRetrieved;
        private dynamic _contactArguments;

        private List<Organisation> _organisations = new List<Organisation>();

        public WhenDeleteContact(RestClient restClient,
            IDbConnection dbconnection)
        {
            _restClient = restClient;
            _dbconnection = dbconnection;
        }

        [When(@"I Delete a Contact")]
        public void WhenIDeleteAContact(IEnumerable<dynamic> contactArguments)
        {
            _contactArguments = contactArguments.First();

            HttpResponseMessage response = _restClient.HttpClient.GetAsync(
                "api/v1/organisations").Result;

            _restClient.Result = response.Content.ReadAsStringAsync().Result;
            _restClient.HttpResponseMessage = response;

            _organisations = JsonConvert.DeserializeObject<List<Organisation>>(_restClient.Result);

            var createContactRequest = new CreateContactRequest
            {
                Username = _contactArguments.UserName,
                DisplayName = _contactArguments.DisplayName,
                Email = _contactArguments.Email,
                EndPointAssessorOrganisationId = _organisations.First().EndPointAssessorOrganisationId
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                 "api/v1/contacts", createContactRequest).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            _restClient.HttpResponseMessage = _restClient.HttpClient.DeleteAsJsonAsync($"api/v1/contacts?username={createContactRequest.Username}").Result;
        }

        [When(@"I Delete a Contact Twice")]
        public void WhenIDeleteAContactTwice(IEnumerable<dynamic> contactArguments)
        {
            _contactArguments = contactArguments.First();

            HttpResponseMessage response = _restClient.HttpClient.GetAsync(
                "api/v1/organisations").Result;

            _restClient.Result = response.Content.ReadAsStringAsync().Result;
            _restClient.HttpResponseMessage = response;

            _organisations = JsonConvert.DeserializeObject<List<Organisation>>(_restClient.Result);

            var createContactRequest = new CreateContactRequest
            {
                Username = _contactArguments.UserName,
                DisplayName = _contactArguments.DisplayName,
                Email = _contactArguments.Email,
                EndPointAssessorOrganisationId = _organisations.First().EndPointAssessorOrganisationId
            };

            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                "api/v1/contacts", createContactRequest).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            _restClient.HttpResponseMessage = _restClient.HttpClient.DeleteAsJsonAsync($"api/v1/contacts?username={createContactRequest.Username}").Result;
            _restClient.HttpResponseMessage = _restClient.HttpClient.DeleteAsJsonAsync($"api/v1/contacts?username={createContactRequest.Username}").Result;
        }

        [Then(@"the Contact should be deleted")]
        public void ThenTheContactShouldBeDeleted()
        {
            var contacts = _dbconnection.Query<Contact>
            ($"Select Status From Contacts where Username = '{_contactArguments.UserName}'").ToList();
            var contact = contacts.First();

            contact.Status.Should().Be(OrganisationStatus.Deleted);
        }

        //[When(@"I Delete an Organisation Twice")]
        //public void WhenIDeleteAnOrganisationTwice(IEnumerable<dynamic> organisations)
        //{
        //    _organisationArguments = organisations.First();

        //    var organisation = new OrganisationCreateViewModel
        //    {
        //        EndPointAssessorName = _organisationArguments.EndPointAssessorName,
        //        EndPointAssessorOrganisationId = _organisationArguments.EndPointAssessorOrganisationId.ToString(),
        //        EndPointAssessorUKPRN = Convert.ToInt32(_organisationArguments.EndPointAssessorUKPRN),
        //        PrimaryContactId = null
        //    };

        //    _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
        //         "api/v1/organisations", organisation).Result;
        //    _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;

        //    var organisationCreated = JsonConvert.DeserializeObject<OrganisationQueryViewModel>(_restClient.Result);

        //    _restClient.HttpResponseMessage = _restClient.HttpClient.DeleteAsJsonAsync($"api/v1/organisations?id={organisationCreated.Id}").Result;
        //    _restClient.HttpResponseMessage = _restClient.HttpClient.DeleteAsJsonAsync($"api/v1/organisations?id={organisationCreated.Id}").Result;
        //}

        //[Then(@"the Organisation should be deleted")]
        //public void ThenTheOrganisationShouldBeDeleted()
        //{
        //    var organisationsCreated = _dbconnection.Query<OrganisationQueryViewModel>
        //    ($"Select EndPointAssessorOrganisationId, EndPointAssessorUKPRN, EndPointAssessorName, OrganisationStatus From Organisations where EndPointAssessorOrganisationId = {_organisationArguments.EndPointAssessorOrganisationId}").ToList();
        //    _organisationRetrieved = organisationsCreated.First();

        //    _organisationRetrieved.OrganisationStatus.Should().Be(OrganisationStatus.Deleted);
        //}

        private void CreateOrganisation(CreateOrganisationRequest organisationCreateViewModel)
        {
            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                "api/v1/organisations", organisationCreateViewModel).Result;
            _restClient.Result = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            _organisationRetrieved = JsonConvert.DeserializeObject<Organisation>(_restClient.Result);
        }
    }
}
