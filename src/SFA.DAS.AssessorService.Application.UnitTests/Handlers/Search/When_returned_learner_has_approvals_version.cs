using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_returned_learner_has_approvals_version : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();

            var certificateId = Guid.NewGuid();
            var searchingEpaoOrgId = Guid.NewGuid();

            CertificateRepository.Setup(r => r.GetDraftAndCompletedCertificatesFor(It.IsAny<long>()))
                .ReturnsAsync(new List<Certificate>());

            StandardService.Setup(s => s.GetAllStandardOptions()).ReturnsAsync(
                new List<StandardOptions> {
                    new StandardOptions
                    {
                        StandardUId = "ST013_1.0",
                        StandardCode = 13,
                        Version = "1.0",
                        StandardReference = "ST013",
                        CourseOption = new List<string>
                        {
                            "MyOption",
                            "MyOption1",
                            "MyOption2"
                        }
                    },
                    new StandardOptions{
                        StandardUId = "ST012_1.0",
                        StandardCode = 12,
                        Version = "1.0",
                        StandardReference = "ST012",
                        CourseOption = new List<string>
                        {
                            "MyOption",
                            "MyOption1",
                            "MyOption2"
                        }
                    }
            });

            LearnerRepository.Setup(r => r.SearchForLearnerByUln(1111111111))
                .ReturnsAsync(new List<Domain.Entities.Learner> {
                    new Domain.Entities.Learner()
                    {
                        StdCode = 13,
                        FamilyName = "Lamora",
                        Version = "1.0",
                        CourseOption = "MyOption",
                        VersionConfirmed = true,
                        StandardUId = "ST013_1.0"}});

            LearnerRepository.Setup(r => r.SearchForLearnerByUln(2222222222))
                .ReturnsAsync(new List<Domain.Entities.Learner> {
                    new Domain.Entities.Learner()
                    {
                        StdCode = 12,
                        FamilyName = "Gamora",
                        Version = "1.0",
                        CourseOption = "",
                        VersionConfirmed = true,
                        StandardUId = "ST012_1.0"}});
        }

        [Test]
        public void Then_a_response_is_returned_with_single_version_and_single_option()
        {
            var result =
                SearchHandler.Handle(
                    new LearnerSearchRequest() { Surname = "Lamora", EpaOrgId = "12345", Uln = 1111111111, Username = "username" },
                    new CancellationToken()).Result;

            result.Count.Should().Be(1);
            result[0].Version.Should().Be("1.0");
            result[0].Option.Should().Be("MyOption");
            result[0].Versions.Count.Should().Be(1);
            result[0].Versions[0].Title.Should().Be("Standard Name 13");
            result[0].Versions[0].StandardUId.Should().Be("ST013_1.0");
            result[0].Versions[0].Version.Should().Be("1.0");
            result[0].Versions[0].Options.Should().BeEquivalentTo(new List<string> { "MyOption" });
        }

        [Test]
        public void Then_a_response_is_returned_with_single_version_and_all_options_if_option_not_set()
        {
            var result =
                SearchHandler.Handle(
                    new LearnerSearchRequest() { Surname = "Gamora", EpaOrgId = "12345", Uln = 2222222222, Username = "username" },
                    new CancellationToken()).Result;

            result.Count.Should().Be(1);
            result[0].Version.Should().Be("1.0");
            result[0].Option.Should().BeNullOrEmpty();
            result[0].Versions.Count.Should().Be(1);
            result[0].Versions[0].Title.Should().Be("Standard Name 12");
            result[0].Versions[0].StandardUId.Should().Be("ST012_1.0");
            result[0].Versions[0].Version.Should().Be("1.0");
            result[0].Versions[0].Options.Should().BeEquivalentTo(new List<string>
            {
                "MyOption",
                "MyOption1",
                "MyOption2"
            });
        }
    }
}