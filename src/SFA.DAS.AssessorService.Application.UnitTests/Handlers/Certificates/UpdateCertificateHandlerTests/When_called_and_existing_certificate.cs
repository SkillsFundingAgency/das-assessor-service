using System;
using System.Collections.Generic;
using System.Threading;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Private;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.Services;
using Organisation = SFA.DAS.AssessorService.Domain.Entities.Organisation;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificateHandlerTests
{
    [TestFixture]
    public class When_called_and_existing_certificate
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private UpdateCertificateHandler _updateCertificateHandler;
        private Guid _certificateId;
        private string _certificateReference;
        private Certificate _returnedCertificate;

        [SetUp]
        public void Arrange()
        {
            _certificateId = Guid.NewGuid();
            _certificateReference = "00010000";
            var existingCertData = Builder<CertificateData>.CreateNew().With(cd => cd.OverallGrade = CertificateGrade.Fail).Build();
            var existingCert = Builder<Certificate>.CreateNew().With(c => c.Id = _certificateId)
                                                               .With(c => c.CertificateReference = _certificateReference)
                                                               .With(c => c.CertificateData = JsonConvert.SerializeObject(existingCertData)).Build();

            var updatedCertData = Builder<CertificateData>.CreateNew().With(cd => cd.OverallGrade = CertificateGrade.Pass).With(cd => cd.AchievementDate = DateTime.Now).Build();
            var epaoRecord = new EpaRecord { EpaDate = updatedCertData.AchievementDate.Value, EpaOutcome = "pass" };
            var epaoDetails = new EpaDetails
            {
                EpaReference = _certificateReference,
                LatestEpaDate = epaoRecord.EpaDate,
                LatestEpaOutcome = epaoRecord.EpaOutcome,
                Epas = new List<EpaRecord> { epaoRecord }
            };
            updatedCertData.EpaDetails = epaoDetails;

            var updatedCert = Builder<Certificate>.CreateNew().With(c => c.Id = _certificateId)
                                                               .With(c => c.CertificateReference = _certificateReference)
                                                               .With(c => c.CertificateData = JsonConvert.SerializeObject(updatedCertData)).Build();


            _certificateRepository = new Mock<ICertificateRepository>();
            _certificateRepository.Setup(r => r.GetCertificateLogsFor(_certificateId)).ReturnsAsync(new List<CertificateLog>());
            _certificateRepository.Setup(r => r.GetCertificate(_certificateId)).ReturnsAsync(existingCert);

            _certificateRepository.Setup(r => r.Update(It.Is<Certificate>(c => c.CertificateReference == _certificateReference), "user", null, true, null))
                .ReturnsAsync(updatedCert);

            _updateCertificateHandler = new UpdateCertificateHandler(_certificateRepository.Object, new Mock<ILogger<UpdateCertificateHandler>>().Object);

            _returnedCertificate = _updateCertificateHandler
                .Handle(new UpdateCertificateRequest(updatedCert) { Username = "user" }, new CancellationToken()).Result;
        }

        [Test]
        public void Then_the_certificate_is_updated()
        {
            _certificateRepository.Verify(r => r.Update(It.Is<Certificate>(c => c.CertificateReference == _certificateReference), "user", null, true, null));
        }

        [Test]
        public void Then_the_EpaDetails_have_been_updated()
        {
            var returnedCertificateData = JsonConvert.DeserializeObject<CertificateData>(_returnedCertificate.CertificateData);
            returnedCertificateData.EpaDetails.EpaReference.Should().Be("00010000");
            returnedCertificateData.EpaDetails.LatestEpaOutcome.Should().Be("pass");
            returnedCertificateData.EpaDetails.Epas.Should().NotBeEmpty();
        }
    }
}
