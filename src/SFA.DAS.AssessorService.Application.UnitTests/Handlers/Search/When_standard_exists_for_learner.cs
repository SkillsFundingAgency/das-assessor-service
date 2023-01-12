using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using OrganisationStandardVersion = SFA.DAS.AssessorService.Api.Types.Models.AO.OrganisationStandardVersion;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_standard_exists_for_learner : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
            CertificateRepository.Setup(r => r.GetDraftAndCompletedCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>());

            LearnerRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Domain.Entities.Learner> { new Domain.Entities.Learner() { StdCode = 12, FamilyName = "Lamora" } });
        }

        [Test]
        public void Then_a_response_is_returned_with_no_certificate_reference()
        {
            var result =
                SearchHandler.Handle(
                    new SearchQuery() { Surname = "Lamora", EpaOrgId = "12345", Uln = 1111111111, Username = "username" },
                    new CancellationToken()).Result;
            result.Count.Should().Be(1);
            result[0].Level.Should().Be(2);
        }

        [Test]
        public void And_epao_not_approved_to_assess_any_standard_version_then_result_is_not_returned()
        {
            StandardService.Setup(s => s.GetEPAORegisteredStandardVersions(It.IsAny<string>(), null))
                .ReturnsAsync(new List<OrganisationStandardVersion> { new OrganisationStandardVersion { Title = "Standard 13", Version = "1.0", LarsCode = 13 } });

            var result =
                SearchHandler.Handle(
                    new SearchQuery() { Surname = "Lamora", EpaOrgId = "12345", Uln = 1111111111, Username = "username" },
                    new CancellationToken()).Result;

            result.Count.Should().Be(0);
        }
    }
}