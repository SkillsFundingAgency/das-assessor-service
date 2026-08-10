using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class When_SearchingByDobAndFamilyName
    {
        private Mock<AssessorDbContext> _mockDbContext;
        private CertificateRepository _sut;

        [SetUp]
        public void SetUp()
        {
            _mockDbContext = new Mock<AssessorDbContext>();

            var mockAssessorUnitOfWork = new Mock<IAssessorUnitOfWork>();
            mockAssessorUnitOfWork
                .SetupGet(x => x.AssessorDbContext)
                .Returns(_mockDbContext.Object);

            _sut = new CertificateRepository(mockAssessorUnitOfWork.Object);
        }

        [Test]
        public async Task Returns_Standard_And_Framework_Matches_And_Respects_Exclude()
        {
            var dateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string familyName = "SMITH";

            var frameworkSearches = new List<FrameworkCertificateSearchResult>
            {
                new()
                {
                    Uln = 1111111111,
                    DateOfBirth = dateOfBirth,
                    CertificateFamilyName = familyName,
                    CourseCode = "T100",
                    CourseName = "Framework A",
                    CourseLevel = "2",
                    DateAwarded = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ProviderName = "ProvA",
                    Ukprn = "12345"
                }
            };

            var standardSearches = new List<StandardCertificateSearchResult>
            {
                new()
                {
                    Uln = 2222222222,
                    DateOfBirth = dateOfBirth,
                    CertificateFamilyName = familyName,
                    CourseCode = "100",
                    CourseName = "Standard A",
                    CourseLevel = "3",
                    DateAwarded = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ProviderName = "ProvStd",
                    Ukprn = 9999
                }
            };

            _mockDbContext
                .Setup(x => x.FrameworkCertificateSearchResults)
                .ReturnsDbSet(frameworkSearches);

            _mockDbContext
                .Setup(x => x.StandardCertificateSearchResults)
                .ReturnsDbSet(standardSearches);

            var results = await _sut.SearchByDobAndFamilyName(
                dateOfBirth,
                familyName,
                new[] { 1111111111L });

            results.Should().BeEquivalentTo(
                [
                    new SearchCertificatesResponse
                    {
                        Uln = 2222222222L,
                        CertificateType = CertificateTypes.Standard,
                        CourseCode = "100",
                        CourseName = "Standard A",
                        CourseLevel = "3",
                        DateAwarded = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        ProviderName = "ProvStd",
                        Ukprn = "9999"
                    }
                ]);
        }

        [Test]
        public async Task Returns_Framework_Match_When_Not_Excluded()
        {
            var dateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string familyName = "SMITH";

            var frameworkSearches = new List<FrameworkCertificateSearchResult>
            {
                new()
                {
                    Uln = 3333333333,
                    DateOfBirth = dateOfBirth,
                    CertificateFamilyName = familyName,
                    CourseCode = "T300",
                    CourseName = "Framework C",
                    CourseLevel = "4",
                    DateAwarded = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ProviderName = "ProvC",
                    Ukprn = "77777"
                }
            };

            _mockDbContext
                .Setup(x => x.FrameworkCertificateSearchResults)
                .ReturnsDbSet(frameworkSearches);

            _mockDbContext
                .Setup(x => x.StandardCertificateSearchResults)
                .ReturnsDbSet(new List<StandardCertificateSearchResult>());

            var results = await _sut.SearchByDobAndFamilyName(
                dateOfBirth,
                familyName,
                null);

            results.Should().BeEquivalentTo(
                [
                    new SearchCertificatesResponse
                    {
                        Uln = 3333333333L,
                        CertificateType = CertificateTypes.Framework,
                        CourseCode = "T300",
                        CourseName = "Framework C",
                        CourseLevel = "4",
                        DateAwarded = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        ProviderName = "ProvC",
                        Ukprn = "77777"
                    }
                ]);
        }

        [Test]
        public async Task Returns_Standard_Match_When_Not_Excluded()
        {
            var dateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string familyName = "SMITH";

            var standardSearches = new List<StandardCertificateSearchResult>
            {
                new()
                {
                    Uln = 4444444444,
                    DateOfBirth = dateOfBirth,
                    CertificateFamilyName = familyName,
                    CourseCode = "200",
                    CourseName = "Standard B",
                    CourseLevel = "2",
                    DateAwarded = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ProviderName = "ProvStd",
                    Ukprn = 8888
                }
            };

            _mockDbContext
                .Setup(x => x.FrameworkCertificateSearchResults)
                .ReturnsDbSet(new List<FrameworkCertificateSearchResult>());

            _mockDbContext
                .Setup(x => x.StandardCertificateSearchResults)
                .ReturnsDbSet(standardSearches);

            var results = await _sut.SearchByDobAndFamilyName(
                dateOfBirth,
                familyName,
                null);

            results.Should().BeEquivalentTo(
                [ 
                    new SearchCertificatesResponse
                    {
                        Uln = 4444444444L,
                        CertificateType = CertificateTypes.Standard,
                        CourseCode = "200",
                        CourseName = "Standard B",
                        CourseLevel = "2",
                        DateAwarded = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        ProviderName = "ProvStd",
                        Ukprn = "8888"
                    } 
                ]);
        }

        [Test]
        public async Task Returns_Matches_Ordered_By_DateAwarded_Descending()
        {
            var dateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string familyName = "SMITH";

            var frameworkSearches = new List<FrameworkCertificateSearchResult>
            {
                new()
                {
                    Uln = 1111111111,
                    DateOfBirth = dateOfBirth,
                    CertificateFamilyName = familyName,
                    DateAwarded = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            };

            var standardSearches = new List<StandardCertificateSearchResult>
            {
                new()
                {
                    Uln = 2222222222,
                    DateOfBirth = dateOfBirth,
                    CertificateFamilyName = familyName,
                    DateAwarded = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            };

            _mockDbContext
                .Setup(x => x.FrameworkCertificateSearchResults)
                .ReturnsDbSet(frameworkSearches);

            _mockDbContext
                .Setup(x => x.StandardCertificateSearchResults)
                .ReturnsDbSet(standardSearches);

            var results = await _sut.SearchByDobAndFamilyName(
                dateOfBirth,
                familyName,
                null);

            results.Select(x => x.Uln)
                .Should()
                .ContainInOrder(2222222222L, 1111111111L);
        }
    }
}