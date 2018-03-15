﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoMapper;
using Dapper;
using FizzWare.NBuilder;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance.Services;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Query.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance
{
    [Binding]
    public sealed class WhenUpdateOrganisation
    {
        private RestClientResult _restClient;
        private readonly OrganisationQueryService _organisationQueryService;
        private readonly OrganisationService _organisationService;
        private readonly OrganisationData _organisationData;
        private readonly ContactData _contactData;
        private readonly ContactQueryService _contactQueryService;
        private readonly UpdateOrganisationRequestBuilder _updateOrganisationRequestBuilder;
        private readonly IDbConnection _dbconnection;
        private OrganisationResponse _organisationResponse;
        private dynamic _organisationArgument;

        public WhenUpdateOrganisation(RestClientResult restClient,
            OrganisationQueryService organisationQueryService,
            OrganisationService organisationService,
            OrganisationData organisationData,
            ContactData contactData,
            ContactQueryService contactQueryService,
            UpdateOrganisationRequestBuilder updateOrganisationRequestBuilder,
            IDbConnection dbconnection)
        {
            _restClient = restClient;
            _organisationQueryService = organisationQueryService;
            _organisationService = organisationService;
            _organisationData = organisationData;
            _contactData = contactData;
            _contactQueryService = contactQueryService;
            _updateOrganisationRequestBuilder = updateOrganisationRequestBuilder;
            _dbconnection = dbconnection;
        }

        [When(@"I Update an Organisation")]
        public void WhenIUpdateAnOrganisation(IEnumerable<dynamic> organisations)
        {
            _organisationArgument = organisations.First();

            //int ukprn = _organisationArgument.EndPointAssessorUKPRN;
            //var restClient = _organisationQueryService.SearchOrganisationByUkPrn(ukprn);
            //var organisation = restClient.Deserialise<OrganisationResponse>();
            //organisation.EndPointAssessorName = _organisationArgument.EndPointAssessorName;


            Organisation organisation = Mapper.Map<Organisation>(_organisationArgument);
            organisation.CreatedAt = DateTime.Now;
            organisation.Status = "New";

            _organisationData.Insert(organisation);

            var updateOrganisationRequest = _updateOrganisationRequestBuilder.Build(organisation);

            _organisationService.PutOrganisation(updateOrganisationRequest);
        }

        [When(@"I Update an Organisation With invalid Id")]
        public void WhenIUpdateAnOrganisationWithInvalidId(IEnumerable<dynamic> organisations)
        {
            var organisation = new UpdateOrganisationRequest
            {
                EndPointAssessorOrganisationId = "9999999999",
                PrimaryContact = null,
                EndPointAssessorName = "XXX"
            };

            _restClient = _organisationService.PutOrganisation(organisation);
        }

        [When(@"I Update an Organisation With Invalid Primary Contact")]
        public void WhenIUpdateAnOrganisationWithInvalidPrimaryContact(IEnumerable<dynamic> organisations)
        {
            _organisationArgument = organisations.First();

            Organisation organisation = Mapper.Map<Organisation>(_organisationArgument);
            organisation.CreatedAt = DateTime.Now;
            organisation.Status = "New";

            _organisationData.Insert(organisation);


            var updateOrganisationRequest = _updateOrganisationRequestBuilder.Build(organisation);
            updateOrganisationRequest.PrimaryContact = "12323";

            _organisationService.PutOrganisation(updateOrganisationRequest);
        }


        [When(@"I Update an Organisation With valid Primary Contact")]
        public void WhenIUpdateAnOrganisationWithValidPrimaryContact(IEnumerable<dynamic> organisations)
        {
            _organisationArgument = organisations.First();

            var contact = Builder<Contact>.CreateNew()
                .With(q => q.EndPointAssessorOrganisationId = "")
                .With(q => q.OrganisationId = null)
                .With(q => q.Username = _organisationArgument.PrimaryContact)
                .Build();

            _contactData.Insert(contact);
          
            Organisation organisation = Mapper.Map<Organisation>(_organisationArgument);
            organisation.CreatedAt = DateTime.Now;
            organisation.Status = "New";

            _organisationData.Insert(organisation);


            var updateOrganisationRequest = _updateOrganisationRequestBuilder.Build(organisation);
            _organisationService.PutOrganisation(updateOrganisationRequest);
        }

        [Then(@"the Update should have occured")]
        public void ThenTheUpdateShouldHaveOccured()
        {
            var organisations
                = _dbconnection.Query<OrganisationResponse>
                    ($"Select EndPointAssessorOrganisationId, EndPointAssessorUKPRN, EndPointAssessorName, Status From Organisations where EndPointAssessorUKPRN = {_organisationArgument.EndPointAssessorUkprn}").ToList();
            _organisationResponse = organisations.First();

            organisations.Count.Should().Equals(1);

            _organisationResponse.EndPointAssessorName.Should().Be(_organisationArgument.EndPointAssessorName);
        }

        [Then(@"the Organisation Status should be persisted as Live")]
        public void ThenTheOrganisationStatusShouldBePersistedAsLive()
        {
            var organisationUpdated = _dbconnection.Query<OrganisationResponse>
              ($"Select EndPointAssessorOrganisationId, EndPointAssessorUKPRN, EndPointAssessorName, Status From Organisations where EndPointAssessorOrganisationId = {_organisationArgument.EndPointAssessorOrganisationId}").ToList();
            _organisationResponse = organisationUpdated.First();

            _organisationResponse.Status.Should().Be(OrganisationStatus.Live);
        }
    }
}
