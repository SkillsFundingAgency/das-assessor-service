﻿using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Search
{
    [TestFixture]
    public class When_search_completed : SearchHandlerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            Setup();

            var certificateId = Guid.NewGuid();
            var searchingEpaoOrgId = Guid.NewGuid();

            CertificateRepository.Setup(r => r.GetDraftAndCompletedCertificatesFor(1111111111))
                .ReturnsAsync(new List<Certificate>
                {
                    new Certificate
                    {
                        Id = certificateId,
                        CertificateReference = "00010001",
                        StandardCode = 12,
                        CertificateData =
                            new CertificateData
                            {
                                OverallGrade = CertificateGrade.Distinction,
                                LearningStartDate = new DateTime(2015, 06, 01),
                                AchievementDate = new DateTime(2018, 06, 01)
                            },
                        IsPrivatelyFunded = false,
                        CreatedBy = "username",
                        CertificateLogs = new List<CertificateLog>
                        {
                            new CertificateLog
                            {
                                CertificateData = new CertificateData
                                {
                                    OverallGrade = CertificateGrade.Distinction,
                                    AchievementDate = new DateTime(2018, 06, 01)
                                },
                                Action = CertificateActions.Submit
                            }
                        }
                    }
                });

            ContactRepository.Setup(cr => cr.GetContact("username"))
                .ReturnsAsync(new Contact() {DisplayName = "EPAO User from this EAPOrg", OrganisationId = searchingEpaoOrgId});

            LearnerRepository.Setup(r => r.SearchForLearnerByUln(It.IsAny<long>()))
                .ReturnsAsync(new List<Domain.Entities.Learner> {new Domain.Entities.Learner() {StdCode = 12, FamilyName = "Lamora"}});
        }                                                           

        [Test]
        public void Then_a_response_is_returned_including_LearnStartDate()
        {
            var result =
                SearchHandler.Handle(
                    new LearnerSearchRequest() {Surname = "Lamora", EpaOrgId= "12345", Uln = 1111111111, Username = "username"},
                    new CancellationToken()).Result;

            result[0].LearnStartDate.Should().Be(new DateTime(2015, 06, 01));
        }

        [Test]
        public void Then_a_Search_Log_entry_is_created()
        {
            SearchHandler.Handle(
                    new LearnerSearchRequest() {Surname = "Lamora", EpaOrgId= "12345", Uln = 1111111111, Username = "username"},
                    new CancellationToken()).Wait();

            LearnerRepository.Verify(r => r.StoreSearchLog(It.Is<SearchLog>(l =>
                l.Username == "username" && 
                l.NumberOfResults == 1 && 
                l.Surname == "Lamora" && 
                l.Uln == 1111111111)));
        }
    }
}