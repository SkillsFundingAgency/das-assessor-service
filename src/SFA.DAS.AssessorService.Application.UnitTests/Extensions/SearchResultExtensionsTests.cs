﻿using FizzWare.NBuilder;
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
    public class SearchResultExtensionsTests
    {
        private List<SearchResult> _searchResults;
        private SearchResult _searchResult;
        private List<Certificate> _certificates;
        private SearchQuery _searchQuery;

        private Contact _searchingContact;
        private Organisation _searchingOrganisation;
        private Certificate _certificate;
        private CertificateData _certificateData;

        private Mock<ICertificateRepository> _mockCertificateRepository;
        private Mock<IContactQueryRepository> _mockContactQueryRepository;
        private Mock<IOrganisationQueryRepository> _mockOrganisationQueryRepository;

        [SetUp]
        public void Arrange()
        {
            _searchQuery = new SearchQuery { Uln = 1111111111 };

            _searchResults = new List<SearchResult>();
            _searchResult = new SearchResult();
            _searchingOrganisation = Builder<Organisation>.CreateNew().Build();

            _mockCertificateRepository = new Mock<ICertificateRepository>();
            _mockContactQueryRepository = new Mock<IContactQueryRepository>();
            _mockOrganisationQueryRepository = new Mock<IOrganisationQueryRepository>();

            SetUpCertificateAndLogEntries();
            SetUpContactQuery();
        }

        [Test]
        public void When_MatchingExistingCompletedStandard_GetCompletedCertificatesByUln()
        {
            MatchUpExistingCompletedStandards();

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

            _searchResult = _searchResults.FirstOrDefault();

            _searchResult.PopulateCertificateBasicInformation(certificate);

            _searchResult.CertificateId.Should().Be(certificate.Id);
            _searchResult.CertificateReference.Should().Be(certificate.CertificateReference);
            _searchResult.CertificateStatus.Should().Be(certificate.Status);
            _searchResult.LearnStartDate.Should().Be(certificateData.LearningStartDate);
            _searchResult.Version.Should().Be(certificateData.Version);
            _searchResult.Option.Should().Be(certificateData.CourseOption);
        }

        [Test]
        public void When_PopulatingExtraCertificateInformation_And_CertificateIsNotSubmitted_Then_ReturnSearchResultsAsIs()
        {
            SetUpIlrRecord();
            _searchResult = _searchResults.FirstOrDefault(r => r.Uln == _searchQuery.Uln);

            PopulateCertificateExtraInformationDependingOnPermission();

            VerifyExtraInformationHasNotBeenAdded();
        }

        [Test]
        public void When_PopulatingExtraCertificateInformation_And_CertificateIsSubmitted_And_SearchingUserIsPartOfOrganisation()
        {
            SetUpIlrRecord();
            SetUpCertificateAndLogEntries(includeSubmitted: true);

            _searchResult = _searchResults.FirstOrDefault(r => r.Uln == _searchQuery.Uln);

            PopulateCertificateExtraInformationDependingOnPermission();

            VerifyExtraInformationHasBeenAdded();
        }

        [Test]
        public void When_PopulatingExtraCertificateInformation_And_CertificateIsSubmitted_And_SearchingUserIsNotPartOfTheOrganisation_Then_ExtraInformationIsNotShown()
        {
            SetUpIlrRecord();
            SetUpCertificateAndLogEntries(includeSubmitted: true);
            _mockContactQueryRepository.Setup(c => c.GetContact(It.Is<string>(username => username != _searchingContact.Username)))
                .ReturnsAsync(new Contact());

            _searchResult = _searchResults.FirstOrDefault(r => r.Uln == _searchQuery.Uln);

            PopulateCertificateExtraInformationDependingOnPermission();

            VerifyExtraInformationHasNotBeenAdded();
        }

        private void MatchUpExistingCompletedStandards()
        {
            _searchResults.MatchUpExistingCompletedStandards(_searchQuery,
                            _mockCertificateRepository.Object,
                            _mockContactQueryRepository.Object,
                            _mockOrganisationQueryRepository.Object,
                            Mock.Of<ILogger<SearchHandler>>());
        }

        private void PopulateCertificateExtraInformationDependingOnPermission()
        {
            _searchResult.PopulateCertificateExtraInformationDependingOnPermission(_searchQuery, _mockCertificateRepository.Object, _mockContactQueryRepository.Object, _certificate, new Organisation(), Mock.Of<ILogger<SearchHandler>>());
        }

        private void VerifyExtraInformationHasBeenAdded()
        {
            _searchResult.ShowExtraInfo.Should().BeTrue();
            _searchResult.OverallGrade.Should().Be(_certificateData.OverallGrade);
            _searchResult.AchDate.Should().Be(_certificateData.AchievementDate);
        }

        private void VerifyExtraInformationHasNotBeenAdded()
        {
            _searchResult.ShowExtraInfo.Should().BeFalse();
            _searchResult.OverallGrade.Should().BeEmpty();
            _searchResult.SubmittedBy.Should().BeEmpty();
            _searchResult.SubmittedAt.Should().BeNull();
            _searchResult.LearnStartDate.Should().BeNull();
            _searchResult.AchDate.Should().BeNull();
            _searchResult.UpdatedBy.Should().BeNull();
            _searchResult.UpdatedAt.Should().BeNull();
        }

        private void SetUpIlrRecord()
        {
            _searchResults = Builder<SearchResult>.CreateListOfSize(1)
                .All()
                    .With(r => r.Uln = 1111111111)
                    .With(r => r.AchDate = null)
                    .With(r => r.OverallGrade = "")
                    .With(r => r.SubmittedBy = "")
                    .With(r => r.SubmittedAt = null)
                    .With(r => r.LearnStartDate = null)
                    .With(r => r.UpdatedAt = null)
                    .With(r => r.UpdatedBy = null)
                    .With(r => r.ShowExtraInfo = false)
                .Build().ToList();
        }

        private void SetUpCertificateAndLogEntries(bool includeSubmitted = false)
        {
            _certificateData = Builder<CertificateData>.CreateNew().Build();

            _certificate = Builder<Certificate>.CreateNew()
                .With(x => x.CertificateData = JsonConvert.SerializeObject(_certificateData))
                .With(r => r.Uln = 1111111111)
            .Build();

            _certificates = new List<Certificate> { _certificate };

            _mockCertificateRepository.Setup(r => r.GetCompletedCertificatesFor(_searchQuery.Uln))
                .ReturnsAsync(_certificates);

            var certificateLogEntries = Builder<CertificateLog>.CreateListOfSize(1)
                .All()
                    .With(x => x.CertificateId = _certificate.Id)
                    .With(x => x.Status = CertificateStatus.Draft)
                    .With(x => x.EventTime = DateTime.UtcNow.AddDays(-1))
                .Build().ToList();
             
            if (includeSubmitted)
            {
                certificateLogEntries.Add(Builder<CertificateLog>.CreateNew()
                    .With(x => x.Status = CertificateStatus.Submitted)
                    .With(x => x.Action = CertificateActions.Submit)
                    .With(x => x.EventTime = DateTime.UtcNow).Build());
            }

            _mockCertificateRepository.Setup(r => r.GetCertificateLogsFor(_certificate.Id))
                .ReturnsAsync(certificateLogEntries);
        }

        private void SetUpContactQuery()
        {
            _searchingContact = Builder<Contact>.CreateNew()
                .With(c => c.OrganisationId = _searchingOrganisation.Id).Build();
            
            _mockContactQueryRepository.Setup(r => r.GetContact(It.IsAny<string>()))
                .ReturnsAsync(_searchingContact);
        }
    }
}