using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance.Services;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using AssessorService.Api.Types.Models;
    using Dapper;
    using Domain.Consts;
    using Extensions;
    using FluentAssertions;
    using Newtonsoft.Json;
    using TechTalk.SpecFlow;

    [Binding]
    public sealed class WhenDeleteOrganisation
    {
        private RestClientResult _restClientResult;
        private readonly CreateOrganisationBuilder _createOrganisationBuilder;
        private readonly OrganisationService _organisationService;
        private readonly IDbConnection _dbconnection;
        private Organisation _organisationRetrieved;
        private dynamic _organisationArgument;

        public WhenDeleteOrganisation(RestClientResult restClientResult,
            CreateOrganisationBuilder createOrganisationBuilder,
            OrganisationService organisationService,
            IDbConnection dbconnection)
        {
            _restClientResult = restClientResult;
            _createOrganisationBuilder = createOrganisationBuilder;
            _organisationService = organisationService;
            _dbconnection = dbconnection;
        }

        [When(@"I Delete an Organisation")]
        public void WhenIDeleteAnOrganisation(IEnumerable<dynamic> organisations)
        {
            _organisationArgument = organisations.First();

            var organisation = _createOrganisationBuilder.Build(_organisationArgument);
            _restClientResult = _organisationService.PostOrganisation(organisation);

            var organisationCreated = _restClientResult.Deserialise<Organisation>();

            _restClientResult = _organisationService.DeleteOrganisation(organisationCreated.EndPointAssessorOrganisationId);
        }

        [When(@"I Delete an Organisation Twice")]
        public void WhenIDeleteAnOrganisationTwice(IEnumerable<dynamic> organisations)
        {
            _organisationArgument = organisations.First();


            var organisation = _createOrganisationBuilder.Build(_organisationArgument);
            _restClientResult = _organisationService.PostOrganisation(organisation);

            var organisationCreated = _restClientResult.Deserialise<Organisation>();

            _restClientResult = _organisationService.DeleteOrganisation(organisationCreated.EndPointAssessorOrganisationId);
            _restClientResult = _organisationService.DeleteOrganisation(organisationCreated.EndPointAssessorOrganisationId);
        }

        [Then(@"the Organisation should be deleted")]
        public void ThenTheOrganisationShouldBeDeleted()
        {
            var organisationsCreated = _dbconnection.Query<Organisation>
            ($"Select EndPointAssessorOrganisationId, EndPointAssessorUKPRN, EndPointAssessorName, Status From Organisations where EndPointAssessorOrganisationId = {_organisationArgument.EndPointAssessorOrganisationId}").ToList();
            _organisationRetrieved = organisationsCreated.First();

            _organisationRetrieved.Status.Should().Be(OrganisationStatus.Deleted);
        }
    }
}
