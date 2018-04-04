using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_certificate_does_not_exist_for_learner : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
            CertificateRepository.Setup(r => r.GetCompletedCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>());

            IlrRepository.Setup(r => r.Search(It.IsAny<SearchRequest>()))
                .ReturnsAsync(new List<Ilr> {new Ilr() {StdCode = "12"}});
        }

        [Test]
        public void Then_a_response_is_returned_with_no_certificate_reference()
        {
            var result =
                SearchHandler.Handle(
                    new SearchQuery() {Surname = "Lamora", UkPrn = 12345, Uln = 1111111111, Username = "username"},
                    new CancellationToken()).Result;
            result.Count.Should().Be(1);
            result[0].CertificateReference.Should().BeNull();
        }
    }
}