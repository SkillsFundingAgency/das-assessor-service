using FizzWare.NBuilder;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using SFA.DAS.AssessorService.Application.Infrastructure;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReferenceDataOrganisationType = SFA.DAS.AssessorService.Api.Types.Models.ReferenceData.OrganisationType;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Orchestrators.OrganisationSearch
{
    public class OrganisationSearchTestBase
    {
        protected Mock<ILogger<OrganisationSearchOrchestrator>> _logger;
        protected Mock<IRoatpApiClient> _roatpApiClient;
        protected Mock<IReferenceDataApiClient> _referenceDataApiClient;
        protected Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            // Arrange
            MappingBootstrapper.Initialize();
            _logger = new Mock<ILogger<OrganisationSearchOrchestrator>>();

            _roatpApiClient = new Mock<IRoatpApiClient>();
            _roatpApiClient
                .Setup(p => p.SearchOrganisationByUkprn(It.IsAny<int>()))
                .ReturnsAsync((int ukprn) => TestOrganisationSearchResultFromRoATP(null, null, ukprn));
            _roatpApiClient
                .Setup(p => p.GetOrganisationByUkprn(It.IsAny<long>()))
                .ReturnsAsync((long ukprn) => TestOrganisationSearchResultFromRoATP(null, null, ukprn).FirstOrDefault());
            _roatpApiClient
                .Setup(p => p.SearchOrganisationByName(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((string name, bool exactMatch) => TestOrganisationSearchResultFromRoATP(name, exactMatch, null));

            _referenceDataApiClient = new Mock<IReferenceDataApiClient>();
            _referenceDataApiClient
                .Setup(p => p.SearchOrgansiation(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync((string name, bool exactMatch) => TestOrganisationSearchResultFromEASAPI(name, exactMatch, null));

            _mediator = new Mock<IMediator>();
            _mediator
                .Setup(p => p.Send(It.IsAny<SearchAssessmentOrganisationsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SearchAssessmentOrganisationsRequest request, CancellationToken token) => TestAssessmentOrganisationSummary(request.SearchTerm).ToList());
            _mediator
                .Setup(p => p.Send(It.IsAny<GetAssessmentOrganisationsByCharityNumbersOrCompanyNumbersRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetAssessmentOrganisationsByCharityNumbersOrCompanyNumbersRequest request, CancellationToken token) => TestAssessmentOrganisationSummaryByCharityNumbers(request.Numbers).ToList());
        }

        static IEnumerable<AssessmentOrganisationSummary> TestAssessmentOrganisationSummary(string searchTerm)
        {
            var allAssessmentOrganisationSummaries = new List<AssessmentOrganisationSummary>
            {
                BuildAssessmentOrganisationSummary(ukprn: 12345678, id: "EPA0001", name: "Blue Barns Limited", legalName: "Blue Barns Limited",
                         companyNumber: "00030001", charityNumber: "", organisationType: "1"),
                BuildAssessmentOrganisationSummary(ukprn: 21345678, id: "EPA0002", name: "Sky Blue Ltd", legalName: "Sky Blue Ltd",
                     companyNumber: "00030002", charityNumber: "", organisationType: "1"),
                BuildAssessmentOrganisationSummary(ukprn: 31245678, id: "EPA0003", name: "Earth Brown Ltd", legalName: "Earth Brown Ltd",
                     companyNumber: "00030003", charityNumber: "00040003", organisationType: "1"),
                BuildAssessmentOrganisationSummary(ukprn: 41235678, id: "EPA0004", name: "Green Grass Limited", legalName: "Green Grass Limited",
                         companyNumber: "00030004", charityNumber: "", organisationType: "1"),
                BuildAssessmentOrganisationSummary(ukprn: 81234567, id: "EPA0008", name: "White Moon Limited", legalName: "White Moon Limited",
                         companyNumber: "00030008", charityNumber: "", organisationType: "1"),
                BuildAssessmentOrganisationSummary(ukprn: 91234567, id: "EPA0009", name: "Purple Flower Ltd", legalName: "Purple Flower Ltd",
                         companyNumber: "00030009", charityNumber: "", organisationType: "1"),
                BuildAssessmentOrganisationSummary(ukprn: 92134567, id: "EPA0010", name: "Large Giving Limited", legalName: "Large Giving Limited",
                         companyNumber: "", charityNumber: "00040010", organisationType: "1"),
                BuildAssessmentOrganisationSummary(ukprn: 93124567, id: "EPA0011", name: "Medium Giving Limited", legalName: "Medium Giving Limited",
                         companyNumber: "", charityNumber: "00040011", organisationType: "1"),
                BuildAssessmentOrganisationSummary(ukprn: 94123567, id: "EPA0012", name: "Small Giving Ltd", legalName: "Small Giving Ltd",
                         companyNumber: "", charityNumber: "00040012", organisationType: "1")
            };

            List<AssessmentOrganisationSummary> matchedAssessmentOrganisationSummaries = new List<AssessmentOrganisationSummary>();

            if (long.TryParse(searchTerm, out long ukprn))
            {
                matchedAssessmentOrganisationSummaries.AddRange(allAssessmentOrganisationSummaries
                    .Where(p => p.Ukprn == ukprn));
            }

            matchedAssessmentOrganisationSummaries.AddRange(allAssessmentOrganisationSummaries
                .Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Id == searchTerm ||
                    p.OrganisationData.CompanyNumber == searchTerm ||
                    p.OrganisationData.CharityNumber == searchTerm));

            return matchedAssessmentOrganisationSummaries;
        }

        static IEnumerable<AssessmentOrganisationSummary> TestAssessmentOrganisationSummaryByCompanyNumbers(IEnumerable<string> companyNumbers)
        {
            List<AssessmentOrganisationSummary> results = new List<AssessmentOrganisationSummary>();

            foreach (var item in companyNumbers)
            {
                results.AddRange(TestAssessmentOrganisationSummary(item));
            }

            return results;
        }

        static IEnumerable<AssessmentOrganisationSummary> TestAssessmentOrganisationSummaryByCharityNumbers(IEnumerable<string> charityNumbers)
        {
            List<AssessmentOrganisationSummary> results = new List<AssessmentOrganisationSummary>();

            foreach (var item in charityNumbers)
            {
                results.AddRange(TestAssessmentOrganisationSummary(item));
            }

            return results;
        }

        static IEnumerable<OrganisationSearchResult> TestOrganisationSearchResultFromRoATP(string name, bool? exactMatch, long? ukprn)
        {
            var organisationSearchResultOne =
                BuildOrganisationSearchResult(organisationReferenceType: "RoATP", roatpApproved: true, ukprn: 12345678, id: "12345678", legalName: "Blue Barns Limited", tradingName: "Blue Barns Limited", providerName: "Blue Barns Limited",
                         companyNumber: "00030001", charityNumber: "", organisationType: "Training Provider");

            var organisationSearchResultSix =
                BuildOrganisationSearchResult(organisationReferenceType: "RoATP", roatpApproved: true, ukprn: 61234578, id: "61234578", legalName: "Yellow Sun Limited", tradingName: "Yellow Sun  Limited", providerName: "Yellow Sun Limited",
                         companyNumber: "00030006", charityNumber: "", organisationType: "Training Provider");

            var allOrganisationSearchResults = new List<OrganisationSearchResult>
            {
                organisationSearchResultOne,
                organisationSearchResultSix
            };

            return (name, exactMatch, ukprn) switch
            {
                { name: not null, ukprn: null } when (!exactMatch.HasValue || !exactMatch.Value) => allOrganisationSearchResults.Where(p => p.Name.Contains(name)),
                { name: not null, ukprn: null } when (exactMatch.HasValue && exactMatch == true) => allOrganisationSearchResults.Where(p => p.Name == name),
                { name: null, ukprn: not null } when !exactMatch.HasValue => allOrganisationSearchResults.Where(p => p.Ukprn == ukprn),
                _ => throw new Exception("Invalid parameters for test setup")
            };
        }

        static IEnumerable<OrganisationSearchResult> TestOrganisationSearchResultFromEASAPI(string name, bool? exactMatch, long? ukprn)
        {
            var allOrganisationSearchResults = new List<OrganisationSearchResult>
            {
                BuildOrganisationSearchResult(organisationReferenceType: "EASAPI", roatpApproved: false, ukprn: null, id: "00030004", legalName: "Green Grass Limited (New Name)", tradingName: "Green Grass Limited (New Name)", providerName: "Green Grass Limited (New Name)",
                         companyNumber: "00030004", charityNumber: "", organisationType: ReferenceDataOrganisationType.Company.ToString()),
                BuildOrganisationSearchResult(organisationReferenceType: "EASAPI", roatpApproved: false, ukprn: null, id: "00030005", legalName: "Red Sky Limited", tradingName: "Red Sky Limited", providerName: "Red Sky Limited",
                         companyNumber: "00030005", charityNumber: "", organisationType: ReferenceDataOrganisationType.EducationOrganisation.ToString()),
                BuildOrganisationSearchResult(organisationReferenceType: "EASAPI", roatpApproved: false, ukprn: null, id: "00030007", legalName: "Green Bush Ltd", tradingName: "Green Bush Ltd", providerName: "Green Bush Ltd",
                         companyNumber: "00030007", charityNumber: "", organisationType: ReferenceDataOrganisationType.Company.ToString()),
                BuildOrganisationSearchResult(organisationReferenceType: "EASAPI", roatpApproved: false, ukprn: null, id: "00030008", legalName: "White Moon Limited (New Name)", tradingName: "White Moon Limited (New Name)", providerName: "White Moon Limited (New Name)",
                         companyNumber: "00030008", charityNumber: "", organisationType: ReferenceDataOrganisationType.Company.ToString()),
                BuildOrganisationSearchResult(organisationReferenceType: "EASAPI", roatpApproved: false, ukprn: null, id: "00030009", legalName: "Purple Flower Ltd (New Name)", tradingName: "Purple Flower Ltd (New Name)", providerName: "Purple Flower Ltd (New Name)",
                         companyNumber: "00030009", charityNumber: "", organisationType: ReferenceDataOrganisationType.Company.ToString()),
                BuildOrganisationSearchResult(organisationReferenceType: "EASAPI", roatpApproved: false, ukprn: null, id: "00040010", legalName: "Large Giving Limited (New Name)", tradingName: "Large Giving Limited (New Name)", providerName: "Large Giving Limited (New Name)",
                         companyNumber: "", charityNumber: "00040010", organisationType: ReferenceDataOrganisationType.Charity.ToString()),
                BuildOrganisationSearchResult(organisationReferenceType: "EASAPI", roatpApproved: false, ukprn: null, id: "00040011", legalName: "Medium Giving Limited (New Name)", tradingName: "Medium Giving Limited (New Name)", providerName: "Medium Giving Limited (New Name)",
                         companyNumber: "", charityNumber: "00040011", organisationType: ReferenceDataOrganisationType.Charity.ToString()),
                BuildOrganisationSearchResult(organisationReferenceType: "EASAPI", roatpApproved: false, ukprn: null, id: "00040012", legalName: "Small Giving Ltd (New Name)", tradingName : "Small Giving Ltd (New Name)", providerName : "Small Giving Ltd (New Name)",
                         companyNumber: "", charityNumber: "00040012", organisationType: ReferenceDataOrganisationType.Charity.ToString()),
            };

            return (name, exactMatch, ukprn) switch
            {
                { name: not null, ukprn: null } when (!exactMatch.HasValue || !exactMatch.Value) => allOrganisationSearchResults.Where(p => p.Name.Contains(name)),
                { name: not null, ukprn: null } when (exactMatch.HasValue && exactMatch == true) => allOrganisationSearchResults.Where(p => p.Name == name),
                { name: null, ukprn: not null } when !exactMatch.HasValue => allOrganisationSearchResults.Where(p => p.Ukprn == ukprn),
                _ => throw new Exception("Invalid parameters for test setup")
            };
        }

        private static AssessmentOrganisationSummary BuildAssessmentOrganisationSummary(long? ukprn, string id, string name, 
            string legalName, string companyNumber, string charityNumber, string organisationType)
        {
            var assessmentOrganisationSummary = Builder<AssessmentOrganisationSummary>
                .CreateNew()
                .With(p => p.Ukprn = ukprn)
                .With(p => p.Id = id)
                .With(p => p.Name = name)
                .With(p => p.OrganisationData =
                    Builder<OrganisationData>
                        .CreateNew()
                        .With(p => p.LegalName = legalName)
                        .With(p => p.Address1 = string.Empty)
                        .With(p => p.Address2 = string.Empty)
                        .With(p => p.Address3 = string.Empty)
                        .With(p => p.Address4 = string.Empty)
                        .With(p => p.Postcode = string.Empty)
                        .With(p => p.CompanyNumber = companyNumber)
                        .With(p => p.CharityNumber = charityNumber)
                        .With(p => p.FHADetails =
                            Builder<FHADetails>
                                .CreateNew()
                                .With(p => p.FinancialDueDate = DateTime.Now.AddYears(1))
                                .With(p => p.FinancialExempt = false)
                                .Build())
                    .Build())
                .With(p => p.Email = string.Empty)
                .With(p => p.OrganisationType = organisationType)
                .With(p => p.Status = OrganisationStatus.Live)
                .Build();

            return assessmentOrganisationSummary;
        }

        private static OrganisationSearchResult BuildOrganisationSearchResult(string organisationReferenceType, bool roatpApproved, 
            int? ukprn, string id,  string legalName, string tradingName, string providerName,
            string companyNumber, string charityNumber, string organisationType)
        {
            var organisationSearchResult = Builder<OrganisationSearchResult>
                .CreateNew()
                .With(p => p.OrganisationReferenceType = organisationReferenceType)
                .With(p => p.RoEPAOApproved = false)
                .With(p => p.RoATPApproved = roatpApproved)
                .With(p => p.Ukprn = ukprn)
                .With(p => p.Id = id)
                .With(p => p.LegalName = legalName)
                .With(p => p.TradingName = tradingName)
                .With(p => p.ProviderName = providerName)
                .With(p => p.Address = 
                    Builder<OrganisationAddress>
                        .CreateNew()
                        .With(p => p.Address1= string.Empty)
                        .With(p => p.Address2 = string.Empty)
                        .With(p => p.Address3 = string.Empty)
                        .With(p => p.City = string.Empty)
                        .With(p => p.Postcode = string.Empty)
                        .Build())
                .With(p => p.CompanyNumber = companyNumber)
                .With(p => p.CharityNumber = charityNumber)
                .With(p => p.FinancialDueDate = DateTime.Now.AddYears(1))
                .With(p => p.FinancialExempt = false)
                .With(p => p.Email = string.Empty)
                .With(p => p.OrganisationType = organisationType)
                .With(p => p.OrganisationIsLive = false)
                .Build();

            return organisationSearchResult;
        }
    }
}
