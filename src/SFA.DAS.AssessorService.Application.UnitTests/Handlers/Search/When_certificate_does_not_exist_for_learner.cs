using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.SubmissionEvents.Types;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_certificate_does_not_exist_for_learner : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
            CertificateRepository.Setup(r => r.GetSubmittedAndDraftCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>());

       
            IlrRepository.Setup(r => r.SearchForLearnerByUlnAndFamilyName(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Ilr>
                {
                    new Ilr()
                    {
                        Id = Guid.NewGuid(), StdCode = 12, FamilyName = "Lamora", Uln = 1111111111,
                        CompletionStatus = CompletionStatus.Completed
                    }
                });

            IlrRepository.Setup(r => r.RefreshFromSubmissionEventData(It.IsAny<Guid>(), It.IsAny<SubmissionEvent>()))
                .Returns(Task.CompletedTask);
            IlrRepository.Setup(r => r.MarkAllUpToDate(It.IsAny<long>())).Returns(Task.CompletedTask);

            ContactRepository.Setup(r => r.GetContact("username")).ReturnsAsync(new Contact
            {
                OrganisationId = Guid.Parse("B79EF91B-DB69-445F-B1EC-007B8C1B5D0D")
            });
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