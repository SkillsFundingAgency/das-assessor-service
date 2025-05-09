﻿using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.DeleteCertificateHandlerTests
{
    [TestFixture]
    class When_called_to_delete_certificate
    {
        private Mock<ICertificateRepository> _certificateRepository;
        private DeleteCertificateHandler _deleteCertificateHandler;
        private DeleteCertificateRequest _deleteCertificateRequest;

        [SetUp]
        public void Arrange()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _certificateRepository.Setup(c => c.Delete(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<string>(), CertificateActions.Delete, true, It.IsAny<string>(), It.IsAny<string>()))
               .Returns(() => Task.Run(() => { })).Verifiable();

            _deleteCertificateHandler = new DeleteCertificateHandler(_certificateRepository.Object);

            _deleteCertificateRequest = new DeleteCertificateRequest
            {
                Uln = 1111111111,
                StandardCode = 30,
                UserName = "User Name",
                ReasonForChange = "Reason for Change",                
                IncidentNumber = "12345"
            };
        }

        [Test]
        public async Task Then_soft_delete_certificate()
        {
            //Act
            var result = await _deleteCertificateHandler.Handle(_deleteCertificateRequest, CancellationToken.None);

            //Assert
            result.Should().NotBeNull();
            _certificateRepository.Verify(c => c.Delete(_deleteCertificateRequest.Uln, _deleteCertificateRequest.StandardCode, _deleteCertificateRequest.UserName, CertificateActions.Delete, true, _deleteCertificateRequest.ReasonForChange, _deleteCertificateRequest.IncidentNumber), Times.Once);
        }
        
    }
}
