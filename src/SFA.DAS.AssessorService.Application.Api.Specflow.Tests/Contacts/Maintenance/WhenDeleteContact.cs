using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Maintenance.Services;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Maintenance
{
    [Binding]
    public sealed class WhenDeleteContact
    {
        private readonly ContactQueryService _contactQueryService;
        private readonly ContactService _contactService;
        private readonly CreateContactBuilder _createContactBuilder;
        private readonly IDbConnection _dbconnection;
        private readonly OrganisationQueryService _organisationQueryService;
        private dynamic _contactArgument;
        private Organisation _organisationRetrieved;

        private List<Organisation> _organisations = new List<Organisation>();
        private RestClientResult _restClient;

        public WhenDeleteContact(RestClientResult restClient,
            OrganisationQueryService organisationQueryService,
            ContactService contactService,
            ContactQueryService contactQueryService,
            CreateContactBuilder createContactBuilder,
            IDbConnection dbconnection)
        {
            _restClient = restClient;
            _organisationQueryService = organisationQueryService;
            _contactService = contactService;
            _contactQueryService = contactQueryService;
            _createContactBuilder = createContactBuilder;
            _dbconnection = dbconnection;
        }

        [When(@"I Delete a Contact")]
        public void WhenIDeleteAContact(IEnumerable<dynamic> contactArguments)
        {
            _contactArgument = contactArguments.First();

            _restClient = _organisationQueryService.GetOrganisations();
            _organisations = _restClient.Deserialise<List<Organisation>>().ToList();

            var createContactRequest = _createContactBuilder.Build(_contactArgument,
                _organisations.First().EndPointAssessorOrganisationId);
            _contactService.PostContact(createContactRequest);

            _contactService.DeleteContact(createContactRequest.Username);
        }

        [When(@"I Delete a Contact Twice")]
        public void WhenIDeleteAContactTwice(IEnumerable<
            dynamic> contactArguments)
        {
            _contactArgument = contactArguments.First();

            _restClient = _organisationQueryService.GetOrganisations();
            _organisations = _restClient.Deserialise<List<Organisation>>().ToList();

            var createContactRequest = _createContactBuilder.Build(_contactArgument,
                _organisations.First().EndPointAssessorOrganisationId);
            _contactService.PostContact(createContactRequest);

            _contactService.DeleteContact(createContactRequest.Username);
            _contactService.DeleteContact(createContactRequest.Username);
        }

        [Then(@"the Contact should be deleted")]
        public void ThenTheContactShouldBeDeleted()
        {
            var contacts = _dbconnection.Query<Contact>
                ($"Select Status From Contacts where Username = '{_contactArgument.UserName}'").ToList();
            var contact = contacts.First();

            contact.Status.Should().Be(OrganisationStatus.Deleted);
        }

        private void CreateOrganisation(CreateOrganisationRequest organisationCreateViewModel)
        {
            _restClient.HttpResponseMessage = _restClient.HttpClient.PostAsJsonAsync(
                "api/v1/organisations", organisationCreateViewModel).Result;
            _restClient.JsonResult = _restClient.HttpResponseMessage.Content.ReadAsStringAsync().Result;

            _organisationRetrieved = JsonConvert.DeserializeObject<Organisation>(_restClient.JsonResult);
        }
    }
}