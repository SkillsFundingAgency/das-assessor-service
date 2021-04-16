using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Infrastructure;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.StartCertificateHandlerTests
{
    [TestFixture]
    public class When_called_and_no_existing_certificate
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private StartCertificateHandler _startCertificateHandler;
        private Guid _organisationId;
        private Certificate _returnedCertificate;
        private Certificate _savedCertificate;

        [SetUp]
        public void Arrange()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _certificateRepository.Setup(r => r.GetCertificate(1111111111, 30)).ReturnsAsync(default(Certificate));

            _certificateRepository.Setup(r => r.New(It.IsAny<Certificate>())).ReturnsAsync(new Certificate() { CertificateReferenceId = 10000 })
                .Callback<Certificate>(c => _savedCertificate = c);

            var ilrRepository = new Mock<IIlrRepository>();
            ilrRepository.Setup(r => r.Get(1111111111, 30)).ReturnsAsync(new Ilr()
            {
                GivenNames = "Dave",
                FamilyName = "Smith",
                StdCode = 30,
                LearnStartDate = new DateTime(2016, 01, 09),
                UkPrn = 12345678,
                FundingModel = 81,
            });

            ilrRepository.Setup(r => r.Get(222222222, 30)).ReturnsAsync(new Ilr()
            {
                GivenNames = "Dave",
                FamilyName = "Smith",
                StdCode = 30,
                LearnStartDate = new DateTime(2016, 01, 09),
                UkPrn = 12345678,
                FundingModel = 99,
            });

            var organisationQueryRepository = new Mock<IOrganisationQueryRepository>();

            _organisationId = Guid.NewGuid();

            organisationQueryRepository.Setup(r => r.GetByUkPrn(88888888)).ReturnsAsync(new Organisation() { Id = _organisationId });

            var roatpApiClientMock = new Mock<IRoatpApiClient>();
            var standardService = new Mock<IStandardService>();

            standardService.Setup(c => c.GetStandardVersionsByLarsCode(30))
                .ReturnsAsync(new List<Standard> { new Standard()
                {
                    Title = "Standard Name",
                    EffectiveFrom = new DateTime(2016,09,01),
                    StandardUId = "ST0016",
                    Version = 1.0m

                } });

            standardService.Setup(c => c.GetStandardOptionsByStandardId("ST0016")).ReturnsAsync(new StandardOptions() { CourseOption = new List<string> { "singleOption" } });

            roatpApiClientMock.Setup(c => c.GetOrganisationByUkprn(It.IsAny<long>()))
                .ReturnsAsync(new OrganisationSearchResult { ProviderName = "A Provider" });

            _startCertificateHandler = new StartCertificateHandler(_certificateRepository.Object,
                ilrRepository.Object, roatpApiClientMock.Object,
                organisationQueryRepository.Object, new Mock<ILogger<StartCertificateHandler>>().Object, standardService.Object);

            _startCertificateHandler
                .Handle(
                    new StartCertificateRequest()
                    {
                        StandardCode = 30,
                        UkPrn = 88888888,
                        Uln = 1111111111,
                        Username = "user"
                    }, new CancellationToken()).Wait();

            _returnedCertificate = _startCertificateHandler
                .Handle(
                    new StartCertificateRequest()
                    {
                        StandardCode = 30,
                        UkPrn = 88888888,
                        Uln = 222222222,
                        Username = "user"
                    }, new CancellationToken()).Result;
        }

        [Test]
        public void Then_a_new_certificate_is_created()
        {
            _certificateRepository.Verify(r => r.New(It.Is<Certificate>(c =>
                c.Uln == 1111111111 &&
                c.StandardCode == 30 &&
                c.ProviderUkPrn == 12345678 &&
                c.OrganisationId == _organisationId &&
                c.CreatedBy == "user" &&
                c.Status == Domain.Consts.CertificateStatus.Draft &&
                c.CertificateReference == "" &&
                c.IsPrivatelyFunded == false &&
                c.StandardUId == "ST0016")));

            var certData = JsonConvert.DeserializeObject<CertificateData>(_savedCertificate.CertificateData);
            certData.CourseOption.Should().Be("singleOption");
            certData.Version.Should().Be("1.0");
            _savedCertificate.StandardUId.Should().Be("ST0016");

        }

        [Test]
        public void Then_a_new_privately_funded_certificate_is_created()
        {
            _certificateRepository.Verify(r => r.New(It.Is<Certificate>(c =>
                c.Uln == 222222222 &&
                c.StandardCode == 30 &&
                c.ProviderUkPrn == 12345678 &&
                c.OrganisationId == _organisationId &&
                c.CreatedBy == "user" &&
                c.Status == Domain.Consts.CertificateStatus.Draft &&
                c.CertificateReference == "" &&
                c.IsPrivatelyFunded == true)));
        }
    }
}