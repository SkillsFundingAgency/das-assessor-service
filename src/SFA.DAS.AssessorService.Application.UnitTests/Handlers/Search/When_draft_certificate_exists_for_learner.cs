﻿using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Threading;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_draft_certificate_exists_for_learner : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();
            CertificateRepository.Setup(r => r.GetDraftAndCompletedCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>()
                {
                    new Certificate
                    {
                        CertificateReference = "00010001",
                        StandardCode = 12,
                        Status = CertificateStatus.Draft
                    }
                });

            IlrRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Ilr> {new Ilr() {StdCode = 12, FamilyName = "Lamora"}});
        }

        [Test]
        public void Then_a_response_is_returned_with_no_certificate_reference()
        {
            var result =
                SearchHandler.Handle(
                    new SearchQuery() {Surname = "Lamora", EpaOrgId= "12345", Uln = 1111111111, Username = "username"},
                    new CancellationToken()).Result;
            result.Count.Should().Be(1);
            result[0].CertificateReference.Should().BeNull();
        }
    }
}