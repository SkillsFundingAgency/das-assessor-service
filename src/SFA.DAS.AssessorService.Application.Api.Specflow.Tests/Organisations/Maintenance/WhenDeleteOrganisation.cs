﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance
{
    [Binding]
    public sealed class WhenDeleteOrganisation
    {
        private readonly CreateOrganisationRequestBuilder _createOrganisationBuilder;
        private readonly IDbConnection _dbconnection;
        private readonly OrganisationService _organisationService;
        private dynamic _organisationArgument;
        private OrganisationResponse _organisationResponse;
        private RestClientResult _restClientResult;

        public WhenDeleteOrganisation(RestClientResult restClientResult,
            CreateOrganisationRequestBuilder createOrganisationBuilder,
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

            var organisationCreated = _restClientResult.Deserialise<OrganisationResponse>();

            _restClientResult =
                _organisationService.DeleteOrganisation(organisationCreated.EndPointAssessorOrganisationId);
        }

        [When(@"I Delete an Organisation Twice")]
        public void WhenIDeleteAnOrganisationTwice(IEnumerable<dynamic> organisations)
        {
            _organisationArgument = organisations.First();


            var organisation = _createOrganisationBuilder.Build(_organisationArgument);
            _restClientResult = _organisationService.PostOrganisation(organisation);

            var organisationCreated = _restClientResult.Deserialise<OrganisationResponse>();

            _restClientResult =
                _organisationService.DeleteOrganisation(organisationCreated.EndPointAssessorOrganisationId);
            _restClientResult =
                _organisationService.DeleteOrganisation(organisationCreated.EndPointAssessorOrganisationId);
        }

        [Then(@"the Organisation should be deleted")]
        public void ThenTheOrganisationShouldBeDeleted()
        {
            var organisationsCreated = _dbconnection.Query<OrganisationResponse>
                    ($"Select EndPointAssessorOrganisationId, EndPointAssessorUKPRN, EndPointAssessorName, Status From Organisations where EndPointAssessorOrganisationId = {_organisationArgument.EndPointAssessorOrganisationId}")
                .ToList();
            _organisationResponse = organisationsCreated.First();

            _organisationResponse.Status.Should().Be(OrganisationStatus.Deleted);
        }
    }
}