using System;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
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

        [SetUp]
        public void Arrange()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _certificateRepository.Setup(r => r.GetCertificate(1111111111, 30)).ReturnsAsync(default(Certificate));

            _certificateRepository.Setup(r => r.New(It.IsAny<Certificate>()))
                .ReturnsAsync(new Certificate() {CertificateReferenceId = 10000});

            var ilrRepository = new Mock<IIlrRepository>();
            ilrRepository.Setup(r => r.Get(1111111111, 30)).ReturnsAsync(new Ilr()
            {
                GivenNames = "Dave",
                FamilyName = "Smith",
                StdCode = 30,
                LearnStartDate = new DateTime(2016, 01, 09),
                UkPrn = 12345678
            });

            var organisationQueryRepository = new Mock<IOrganisationQueryRepository>();

            _organisationId = Guid.NewGuid();

            organisationQueryRepository.Setup(r => r.GetByUkPrn(88888888)).ReturnsAsync(new Organisation() { Id = _organisationId});

            var assessmentOrgsApiClient = new Mock<IAssessmentOrgsApiClient>();
            assessmentOrgsApiClient.Setup(c => c.GetStandard(30))
                .ReturnsAsync(new Standard() {Title = "Standard Name", EffectiveFrom = new DateTime(2016,09,01)});
            assessmentOrgsApiClient.Setup(c => c.GetProvider(It.IsAny<long>()))
                .ReturnsAsync(new Provider {ProviderName = "A Provider"});

            _startCertificateHandler = new StartCertificateHandler(_certificateRepository.Object,
                ilrRepository.Object, assessmentOrgsApiClient.Object,
                organisationQueryRepository.Object, new Mock<ILogger<StartCertificateHandler>>().Object);

            _returnedCertificate = _startCertificateHandler
                .Handle(
                    new StartCertificateRequest()
                    {
                        StandardCode = 30,
                        UkPrn = 88888888,
                        Uln = 1111111111,
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
                c.CertificateReference == "")));
        }

        [Test]
        public void Then_the_reference_number_is_padded_to_8_characters_with_zeroes()
        {
            _returnedCertificate.CertificateReference.Should().Be("00010000");
            _certificateRepository.Verify(r => r.Update(It.Is<Certificate>(c => c.CertificateReference == "00010000"), "user", null, true, null));
        }
    }
}