using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance
{
    [Binding]
    public sealed class WhenCreateOrganisation
    {
        private RestClientResult _restClientResult;
        private readonly OrganisationService _organisationService;
        private readonly ContactQueryService _contactQueryService;
        private readonly CreateOrganisationBuilder _createOrganisationBuilder;
        private readonly IDbConnection _dbconnection;
        private OrganisationResponse _organisationResponse;
        private dynamic _organisationArgument;

        public WhenCreateOrganisation(RestClientResult restClientResult,
            OrganisationService organisationService,
            ContactQueryService contactQueryService,
            CreateOrganisationBuilder createOrganisationBuilder,
            IDbConnection dbconnection)
        {
            _restClientResult = restClientResult;
            _organisationService = organisationService;
            _contactQueryService = contactQueryService;
            _createOrganisationBuilder = createOrganisationBuilder;
            _dbconnection = dbconnection;
        }

        [When(@"I Create an Organisation")]
        public void WhenICreateAnOrganisation(IEnumerable<dynamic> organisations)
        {
            _organisationArgument = organisations.First();

            var organisation = _createOrganisationBuilder.Build(_organisationArgument);
            _restClientResult = _organisationService.PostOrganisation(organisation);
        }

        [When(@"I Create an Organisation With Existing Primary Contact")]
        public void WhenICreateAnOrganisationWithExistingPrimaryContact(IEnumerable<dynamic> organisations)
        {
            _organisationArgument = organisations.First();

            var contact = CreateContact();
            var organisation = _createOrganisationBuilder.Build(_organisationArgument, contact.Username);

            _restClientResult = _organisationService.PostOrganisation(organisation);
        }

        [Then(@"the Organisation should be created")]
        public void ThenTheOrganisationShouldBeCreated()
        {
            var organisationsCreated = _dbconnection.Query<OrganisationResponse>
              ($"Select EndPointAssessorOrganisationId, EndPointAssessorUKPRN, EndPointAssessorName, Status From Organisations where EndPointAssessorOrganisationId = {_organisationArgument.EndPointAssessorOrganisationId}").ToList();
            _organisationResponse = organisationsCreated.First();

            organisationsCreated.Count.Should().Be(1);

            _organisationResponse.EndPointAssessorOrganisationId.Should().Equals(_organisationArgument.EndPointAssessorOrganisationId);
            _organisationResponse.EndPointAssessorUkprn.Should().Equals(_organisationArgument.EndPointAssessorUKPRN);
            _organisationResponse.EndPointAssessorName.Should().Equals(_organisationArgument.EndPointAssessorName);
        }

        [Then(@"the Organisation Status should be set to (.*)")]
        public void ThenTheOrganisationStatusShouldBeSetTo(string p0)
        {
            switch (p0)
            {
                case "Live":
                    _organisationResponse.Status.Should().Be(OrganisationStatus.Live);
                    break;
                case "New":
                    _organisationResponse.Status.Should().Be(OrganisationStatus.New);
                    break;
                default:
                    throw new ApplicationException("Uknown OrganisationStatus");
            }
        }

        private ContactResponse CreateContact()
        {
            var contactResult = _contactQueryService.SearchForContactByUserName("jcoxhead");
            var contact = contactResult.Deserialise<ContactResponse>();
            return contact;
        }
    }
}
