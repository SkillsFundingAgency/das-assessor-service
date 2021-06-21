using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Search;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.UnitTests.Extensions
{
    public class SearchResultExtensions
    {
        private List<SearchResult> _searchResults;
        private List<Certificate> _certificates;
        private SearchQuery _searchQuery;
        private Mock<ICertificateRepository> _mockCertificateRepository;
        private Mock<IContactQueryRepository> _mockContactQueryRepository;
        private Mock<IOrganisationQueryRepository> _mockOrganisationQueryRepository;

        [SetUp]
        public void Arrange()
        {
            _searchQuery = new SearchQuery { Uln = 1111111111 };

            _searchResults = new List<SearchResult>();

            _mockCertificateRepository = new Mock<ICertificateRepository>();
            _mockContactQueryRepository = new Mock<IContactQueryRepository>();
            _mockOrganisationQueryRepository = new Mock<IOrganisationQueryRepository>();

            SetUpCertificateAndLogEntries();
            SetUpContactQuery();
        }

        [Test]
        public void When_MatchingExistingCompletedStandard_GetCompletedCertificatesByUln()
        {
          
                _searchResults.MatchUpExistingCompletedStandards(_searchQuery,
                                _mockCertificateRepository.Object,
                                _mockContactQueryRepository.Object,
                                _mockOrganisationQueryRepository.Object,
                                Mock.Of<ILogger<SearchHandler>>());
            
            _mockCertificateRepository.Verify(r => r.GetCompletedCertificatesFor(_searchQuery.Uln), Times.Once);
        }

        [Test]
        public void When_MatchingExistingCompletedStandard_And_ResultsFoundByIlr_Then_ReturnResults()
        {
            SetUpIlrRecord();

            MatchUpExistingCompletedStandards();

            _searchResults.Count.Should().Be(1);
        }

        [Test]
        public void When_MatchingExistingCompletedStandard_And_ResultFoundByCertificate_Then_AddCertificateToSearchResults()
        {
            MatchUpExistingCompletedStandards();

            _searchResults.Count.Should().Be(1);
        }

        [Test]
        public void When_PopulatingBasicCertificateInformation_Then_BasicCertificateInformationShouldBeMapped()
        {
            SetUpIlrRecord();

            var certificate = _certificates.FirstOrDefault(c => c.Uln == _searchResults.First().Uln);
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            var searchResult = _searchResults.FirstOrDefault();
                
            searchResult.PopulateCertificateBasicInformation(certificate);

            searchResult.CertificateId.Should().Be(certificate.Id);
            searchResult.CertificateReference.Should().Be(certificate.CertificateReference);
            searchResult.CertificateStatus.Should().Be(certificate.Status);
            searchResult.LearnStartDate.Should().Be(certificateData.LearningStartDate);
            searchResult.Version.Should().Be(certificateData.Version);
            searchResult.Option.Should().Be(certificateData.CourseOption);
        }

        private void MatchUpExistingCompletedStandards()
        {
            _searchResults.MatchUpExistingCompletedStandards(_searchQuery,
                            _mockCertificateRepository.Object,
                            _mockContactQueryRepository.Object,
                            _mockOrganisationQueryRepository.Object,
                            Mock.Of<ILogger<SearchHandler>>());
        }

        private void SetUpIlrRecord()
        {
            _searchResults = Builder<SearchResult>.CreateListOfSize(1)
                .All()
                    .With(r => r.Uln = 1111111111)
                .Build().ToList();
        }

        private void SetUpCertificateAndLogEntries()
        {
            var certificateData = JsonConvert.SerializeObject(Builder<CertificateData>.CreateNew().Build());

            var certificate = Builder<Certificate>.CreateNew()
                .With(x => x.CertificateData = certificateData)
                .With(r => r.Uln = 1111111111)
            .Build();

            _certificates = new List<Certificate> { certificate };

            _mockCertificateRepository.Setup(r => r.GetCompletedCertificatesFor(_searchQuery.Uln))
                .ReturnsAsync(_certificates);

            var certificateLogEntries = Builder<CertificateLog>.CreateListOfSize(2)
                .All()
                    .With(x => x.CertificateId = certificate.Id)
                .TheFirst(1)
                    .With(x => x.Status = CertificateStatus.Draft)
                    .With(x => x.EventTime = DateTime.UtcNow.AddDays(-1))
                .TheNext(1)
                    .With(x => x.Status = CertificateStatus.Submitted)
                    .With(x => x.Action = CertificateActions.Submit)
                    .With(x => x.EventTime = DateTime.UtcNow)
                .Build().ToList();

            _mockCertificateRepository.Setup(r => r.GetCertificateLogsFor(certificate.Id))
                .ReturnsAsync(certificateLogEntries);
        }

        private void SetUpContactQuery()
        {
            var contact = Builder<Contact>.CreateNew().Build();
            
            _mockContactQueryRepository.Setup(r => r.GetContact(It.IsAny<string>()))
                .ReturnsAsync(contact);
        }
    }
}
