using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Standards;
using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Standards
{
    public class WhenHandlingGetCalculatedStandardVersionForApprenticeshipHandler
    {
        [Test, MoqAutoData]
        public async Task ThenSelectsCorrectVersionBasedOnILRLearnerStartDate(
            [Frozen] Mock<IStandardService> standardService,
            [Frozen] Mock<IIlrRepository> ilrRepository,
            GetCalculatedStandardVersionForApprenticeshipRequest request,
            IEnumerable<Standard> standards,
            DateTime baseDate,
            int baseVersion,
            int larsCode,
            Ilr ilrRecord,
            GetCalculatedStandardVersionForApprenticeshipHandler sut)
        {
            //Arrange
            //List defaults to 3 records in autofixture
            // Set learner date for middle version
            ilrRecord.LearnStartDate = baseDate.AddYears(1);
            var selectedVersionNumber = baseVersion + 1;

            foreach (var s in standards)
            {
                s.Version = baseVersion;
                s.VersionLatestStartDate = baseDate;
                s.LarsCode = larsCode;

                baseVersion++;
                baseDate = baseDate.AddYears(1);
            }

            var latestStandard = standards.OrderByDescending(s => s.Version).First();
            var selectedVersion = standards.First(s => s.Version == selectedVersionNumber);

            standardService.Setup(s => s.GetStandardVersionById(request.StandardId, null)).ReturnsAsync(latestStandard);
            standardService.Setup(s => s.GetStandardVersionsByLarsCode(larsCode)).ReturnsAsync(standards);
            ilrRepository.Setup(s => s.Get(request.Uln, larsCode)).ReturnsAsync(ilrRecord);

            //Act
            var result = await sut.Handle(request, new CancellationToken());

            //Assert
            latestStandard.Should().NotBeEquivalentTo(selectedVersion);
            result.Should().BeEquivalentTo(selectedVersion);
        }

        [Test, MoqAutoData]
        public async Task ThenSelectsCorrectVersionBasedOnILRLearnerStartDate_WhenLatestVersionStartDateIsNull(
            [Frozen] Mock<IStandardService> standardService,
            [Frozen] Mock<IIlrRepository> ilrRepository,
            GetCalculatedStandardVersionForApprenticeshipRequest request,
            IEnumerable<Standard> standards,
            DateTime baseDate,
            int baseVersion,
            int larsCode,
            Ilr ilrRecord,
            GetCalculatedStandardVersionForApprenticeshipHandler sut)
        {
            //Arrange
            //List defaults to 3 records in autofixture
            // Set learner date for last version
            ilrRecord.LearnStartDate = baseDate.AddYears(2);
            
            foreach (var s in standards)
            {
                s.Version = baseVersion;
                s.VersionLatestStartDate = baseDate;
                s.LarsCode = larsCode;
                
                baseVersion++;
                baseDate = baseDate.AddYears(1);
            }

            // Set latest Standard EndDate to null
            var latestVersion = standards.OrderByDescending(d => d.Version).First();
            latestVersion.VersionLatestStartDate = null;

            standardService.Setup(s => s.GetStandardVersionById(request.StandardId, null)).ReturnsAsync(latestVersion);
            standardService.Setup(s => s.GetStandardVersionsByLarsCode(larsCode)).ReturnsAsync(standards);
            ilrRepository.Setup(s => s.Get(request.Uln, larsCode)).ReturnsAsync(ilrRecord);

            //Act
            var result = await sut.Handle(request, new CancellationToken());

            //Assert
            result.Should().BeEquivalentTo(latestVersion);
        }

        [Test, MoqAutoData]
        public async Task ThenSelectsLatestVersion_BasedOnILRLearnerStartDateNotMatchingAnyVersion(
            [Frozen] Mock<IStandardService> standardService,
            [Frozen] Mock<IIlrRepository> ilrRepository,
            GetCalculatedStandardVersionForApprenticeshipRequest request,
            IEnumerable<Standard> standards,
            DateTime baseDate,
            int baseVersion,
            int larsCode,
            Ilr ilrRecord,
            GetCalculatedStandardVersionForApprenticeshipHandler sut)
        {
            //Arrange
            //List defaults to 3 records in autofixture
            // Set learner date for last version
            ilrRecord.LearnStartDate = baseDate.AddYears(25);

            foreach (var s in standards)
            {
                s.Version = baseVersion;
                s.VersionLatestStartDate = baseDate;
                s.LarsCode = larsCode;

                baseVersion++;
                baseDate = baseDate.AddYears(1);
            }

            var latestVersion = standards.OrderByDescending(d => d.Version).First();
            
            standardService.Setup(s => s.GetStandardVersionById(request.StandardId, null)).ReturnsAsync(latestVersion);
            standardService.Setup(s => s.GetStandardVersionsByLarsCode(larsCode)).ReturnsAsync(standards);
            ilrRepository.Setup(s => s.Get(request.Uln, larsCode)).ReturnsAsync(ilrRecord);

            //Act
            var result = await sut.Handle(request, new CancellationToken());

            //Assert
            result.Should().BeEquivalentTo(latestVersion);
        }
    }
}
