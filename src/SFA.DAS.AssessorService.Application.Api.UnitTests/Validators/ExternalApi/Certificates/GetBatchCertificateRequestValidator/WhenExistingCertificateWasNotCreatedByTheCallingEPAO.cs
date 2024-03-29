﻿using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Certificates.GetBatchCertificateRequestValidator
{
    public class WhenExistingCertificateWasNotCreatedByTheCallingEPAO : GetBatchCertificateRequestValidatorTestBase
    {
        private GetBatchCertificateRequest _request;
        private ValidationResult _validationResult;

        [SetUp]
        public async Task Arrange()
        {
             _request = Builder<GetBatchCertificateRequest>.CreateNew()
                .With(i => i.Uln = 1234567890)
                .With(i => i.StandardCode = 101)
                .With(i => i.StandardReference = null)
                .With(i => i.UkPrn = 99999999)
                .With(i => i.FamilyName = "Test")
                .Build();

            _validationResult = await Validator.ValidateAsync(_request);
        }

        [Test]
        public void Then_ValidationResultShouldBeTrue()
        {
            _validationResult.IsValid.Should().BeTrue();
        }

        [Test]
        public void Then_GetLearner_GetCertificate_GetEpaoStandards_AreCalledOnce()
        {
            LearnerRepositoryMock.Verify(repo => repo.Get(_request.Uln, _request.StandardCode), Times.Once());
            CertificateRepositoryMock.Verify(repo => repo.GetCertificate(_request.Uln, _request.StandardCode), Times.Once());
            StandardServiceMock.Verify(service => service.GetEpaoRegisteredStandards("99999999"), Times.Once());
        }
    }
}
