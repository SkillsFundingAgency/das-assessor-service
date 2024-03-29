﻿using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.ExternalApi.Certificates.GetBatchCertificateRequestValidator
{
    public class WhenExistingCertificateWasCreatedByTheCallingEPAO : GetBatchCertificateRequestValidatorTestBase
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
                .With(i => i.UkPrn = 12345678)
                .With(i => i.FamilyName = "Test")
                .Build();

            var organisationResponse = Builder<Organisation>.CreateNew().Build();
            var certificateResponse = Builder<Certificate>.CreateNew().Build();

            OrganisationQueryRepositoryMock.Setup(o => o.GetByUkPrn(_request.UkPrn))
                .ReturnsAsync(organisationResponse);

            CertificateRepositoryMock.Setup(c => c.GetCertificate(_request.Uln, _request.StandardCode, _request.FamilyName,
                organisationResponse.EndPointAssessorOrganisationId))
                .ReturnsAsync(certificateResponse);

            _validationResult = await Validator.ValidateAsync(_request);
        }

        [Test]
        public void ThenValidationResultShouldBeTrue()
        {
            _validationResult.IsValid.Should().BeTrue();
        }

        [Test]
        public void ThenGetCertificateIsNotCalled()
        {
            CertificateRepositoryMock.Verify(repo => repo.GetCertificate(_request.Uln, _request.StandardCode), Times.Never());
        }

        [Test]
        public void ThenGetEpaoRegisteredStandardsIsNotCalled()
        {
            StandardServiceMock.Verify(service => service.GetEpaoRegisteredStandards("99999999"), Times.Never());
        }
    }
}
