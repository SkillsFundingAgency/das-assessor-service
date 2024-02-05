using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.GetCertificatesHistoryHandlerTests
{
    public class When_called_to_get_certificates
    {
        private Mock<ICertificateRepository> _certificateRepositoryMock;
        private Mock<IRoatpApiClient> _roatpApiClientMock;
        private Mock<IContactQueryRepository> _contactQueryRepositoryMock;
        private Mock<ILogger<GetCertificatesHistoryHandler>> _loggermock;

        private PaginatedList<CertificateSummaryResponse> _result;

        private const string SubmitUsername = "submit.user@company.com";
        private const string SubmitDisplayname = "SubmitUser";
        private const int ResultPageSize = 10;
        private readonly DateTime SubmitDateTime = DateTime.Now;

        [SetUp]
        public void Arrange()
        {
            MappingBootstrapper.Initialize();

            var certificates = Builder<CertificateHistoryModel>.CreateListOfSize(10)
                .All()
                .With(x => x.CertificateLogs = Builder<CertificateLog>.CreateListOfSize(2)
                    .TheFirst(1)
                        .With(q => q.Status = CertificateStatus.Submitted)
                        .With(q => q.Action = CertificateActions.Submit)
                        .With(q => q.Username = SubmitUsername)
                        .With(q => q.EventTime = SubmitDateTime)
                    .TheNext(1)
                        .With(q => q.Status = CertificateStatus.Submitted)
                        .With(q => q.Action = CertificateActions.Status)
                        .With(q => q.Username = SystemUsers.PrintFunction)
                        .With(q => q.EventTime = SubmitDateTime.AddDays(1))
                    .Build().ToList())
                .Build().ToList();

            _certificateRepositoryMock = new Mock<ICertificateRepository>();
            _certificateRepositoryMock
                .Setup(r => r.GetCertificateHistory(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(new PaginatedList<CertificateHistoryModel>(certificates, 40, 1, ResultPageSize));

            _contactQueryRepositoryMock = new Mock<IContactQueryRepository>();
            _contactQueryRepositoryMock.Setup(r => r.GetContact(SubmitUsername)).ReturnsAsync(new Contact
            {
                DisplayName = SubmitDisplayname
            });

            _roatpApiClientMock = new Mock<IRoatpApiClient>();
            _roatpApiClientMock.Setup(r => r.GetOrganisationByUkprn(It.IsAny<long>()))
                .ReturnsAsync(new OrganisationSearchResult
                {
                    ProviderName = "TestProvider",
                    Ukprn = 123456789
                });

            _loggermock = new Mock<ILogger<GetCertificatesHistoryHandler>>();

            var sut =
                new GetCertificatesHistoryHandler(_certificateRepositoryMock.Object,
                    _roatpApiClientMock.Object, _contactQueryRepositoryMock.Object,
                    _loggermock.Object);

            _result = sut.Handle(new GetCertificateHistoryRequest
            {
                PageIndex = 1,
                EndPointAssessorOrganisationId = "12345677"
            }, new CancellationToken())
                .Result;
        }

        [Test]
        public void Then_certificates_are_returned()
        {
            _result.Items.Count().Should().BeGreaterOrEqualTo(ResultPageSize);
        }

        [Test]
        public void Then_recorded_by_has_submit_user()
        {
            _result.Items.ForEach(x => x.RecordedBy.Should().Be(SubmitDisplayname));
        }
    }
}