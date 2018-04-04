using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using FizzWare.NBuilder;
using FluentAssertions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Contacts.Query;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Helpers;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Organisations.Maintenance
{
    [Binding]
    public sealed class WhenCreateOrganisation
    {
        private RestClientResult _restClientResult;
        private readonly OrganisationService _organisationService;
        private readonly ContactQueryService _contactQueryService;
        private readonly CreateOrganisationRequestBuilder _createOrganisationRequestBuilder;
        private readonly OrganisationData _organisationData;
        private readonly ContactData _contactData;
        private readonly IDbConnection _dbconnection;
        private OrganisationResponse _organisationResponse;
        private dynamic _organisationArgument;

        public WhenCreateOrganisation(RestClientResult restClientResult,
            OrganisationService organisationService,
            ContactQueryService contactQueryService,
            CreateOrganisationRequestBuilder createOrganisationRequestBuilder,
            OrganisationData organisationData,
            ContactData contactData,
            IDbConnection dbconnection)
        {
            _restClientResult = restClientResult;
            _organisationService = organisationService;
            _contactQueryService = contactQueryService;
            _createOrganisationRequestBuilder = createOrganisationRequestBuilder;
            _organisationData = organisationData;
            _contactData = contactData;
            _dbconnection = dbconnection;
        }

        [When(@"I Create an Organisation")]
        public void WhenICreateAnOrganisation(IEnumerable<dynamic> organisations)
        {
            _organisationArgument = organisations.First();

            var createOrganisationRequest = _createOrganisationRequestBuilder.Build(_organisationArgument);
            _restClientResult = _organisationService.PostOrganisation(createOrganisationRequest);
        }

        [Then(@"the Organisation should be created")]
        public void ThenTheOrganisationShouldBeCreated()
        {
            var organisationsCreated = _dbconnection.Query<OrganisationResponse>
              ($"Select EndPointAssessorOrganisationId, EndPointAssessorUKPRN, EndPointAssessorName, Status From Organisations where EndPointAssessorOrganisationId = {_organisationArgument.EndPointAssessorOrganisationId}").ToList();
            _organisationResponse = organisationsCreated.First();

            organisationsCreated.Count.Should().Be(1);

            _organisationResponse.EndPointAssessorOrganisationId.Should().Equals(_organisationArgument.EndPointAssessorOrganisationId);
            _organisationResponse.EndPointAssessorUkprn.Should().Equals(_organisationArgument.EndPointAssessorUkprn);
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

        private Contact CreateContact(string userName)
        {
            var contact = Builder<Contact>.CreateNew()
                .With(q => q.EndPointAssessorOrganisationId = "")
                .With(q => q.OrganisationId = null)
                .With(q => q.Username = userName)
                .Build();

            _contactData.Insert(contact);

            return contact;
        }
    }
}
