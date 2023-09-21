using System;
using System.Threading;
using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Infrastructure;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.StartCertificateHandlerTests
{
    [TestFixture]
    public class When_called_and_certificate_exists_with_delete_status
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private StartCertificateHandler _startCertificateHandler;
        private Guid _organisationId;
        private Certificate _returnedCertificate;
        private CertificateData _certificateData;

        [SetUp]
        public void Arrange()
        {
            var organisationQueryRepository = new Mock<IOrganisationQueryRepository>();
            _organisationId = Guid.NewGuid();
            organisationQueryRepository.Setup(r => r.GetByUkPrn(88888888)).ReturnsAsync(new Organisation() { Id = _organisationId });


            _certificateRepository = new Mock<ICertificateRepository>();

            _certificateData = Builder<CertificateData>.CreateNew()
              .With(ecd => ecd.LearnerGivenNames = "Dave")
              .With(ecd => ecd.LearnerFamilyName = "Smith")
              .With(ecd => ecd.LearningStartDate = new DateTime(2016, 01, 09))
              .Build();

            _certificateRepository.Setup(r => r.GetCertificate(1111111111, 30)).ReturnsAsync(new Certificate()
            {
                CertificateReferenceId = 10000,
                Status = CertificateStatus.Deleted,
                CertificateData = JsonConvert.SerializeObject(_certificateData)
            });

            _certificateRepository.Setup(r => r.Update(It.IsAny<Certificate>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(new Certificate()
            {
                CertificateReferenceId = 10000,
                Status = CertificateStatus.Deleted,
                OrganisationId = _organisationId,
                CertificateData = JsonConvert.SerializeObject(_certificateData)
            });

            var learnerRepository = new Mock<ILearnerRepository>();
            learnerRepository.Setup(r => r.Get(1111111111, 30)).ReturnsAsync(new Domain.Entities.Learner()
            {
                GivenNames = "Dave",
                FamilyName = "Smith",
                StdCode = 30,
                LearnStartDate = new DateTime(2016, 01, 09),
                UkPrn = 12345678
            });

            var standardService = new Mock<IStandardService>();
            standardService.Setup(s => s.GetStandardVersionsByLarsCode(30)).ReturnsAsync(new[] { new Standard { Title = "Standard Title" } });

            var certificateNameCapitalisationService = new Mock<ICertificateNameCapitalisationService>();

            var mockProvidersRepository = new Mock<IProvidersRepository>();
            mockProvidersRepository.Setup(m => m.GetProvider(12345678)).ReturnsAsync(new Provider() { Ukprn = 12345678, Name = "Test Provider Name" });

            _startCertificateHandler = new StartCertificateHandler(_certificateRepository.Object,
                learnerRepository.Object, mockProvidersRepository.Object,
                organisationQueryRepository.Object, new Mock<IStandardRepository>().Object, new Mock<ILogger<StartCertificateHandler>>().Object, standardService.Object, certificateNameCapitalisationService.Object);

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
        public void Then_certificate_is_updated()
        {
            //Assert
            _certificateRepository.Verify(v => v.Update(
            It.Is<Certificate>(c => c.CertificateReferenceId == 10000 && c.OrganisationId == _organisationId),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<string>()),
            Times.Once);
        }
    }
}
