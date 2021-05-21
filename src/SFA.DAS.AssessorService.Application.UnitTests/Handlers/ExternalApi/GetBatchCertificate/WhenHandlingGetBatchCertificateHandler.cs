using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.ExternalApi.GetBatchCertificate
{
    public class WhenHandlingGetBatchCertificateHandler
    {
        private GetBatchCertificateRequest _request;

        private Mock<ICertificateRepository> _mockCertificateRepository;
        private Mock<IStandardRepository> _mockStandardRepository;
        private Mock<IOrganisationQueryRepository> _mockOrganisationQueryRepository;

        private CertificateData _certificateData;
        
        private Certificate _certResponse;
        private Organisation _organisationResponse;
        private IList<CertificateLog> _certificateLogs;
        private EpoRegisteredStandardsResult _registeredStandards;

        private GetBatchCertificateHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _request = Builder<GetBatchCertificateRequest>.CreateNew().Build();

            _certificateData = Builder<CertificateData>.CreateNew()
                .With(d => d.LearnerFamilyName = _request.FamilyName)
                .With(d => d.EpaDetails = Builder<EpaDetails>.CreateNew()
                    .With(e => e.LatestEpaOutcome = "Pass")
                    .Build())
                .Build();

            var certDataJson = JsonConvert.SerializeObject(_certificateData);

            _organisationResponse = Builder<Organisation>.CreateNew().Build();

            _certResponse = Builder<Certificate>.CreateNew()
                .With(c => c.Status = "Submitted")
                .With(c => c.CertificateData = certDataJson)
                .Build();

            _registeredStandards = Builder<EpoRegisteredStandardsResult>.CreateNew()
                .With(s => s.PageOfResults = new List<EPORegisteredStandards> {
                    new EPORegisteredStandards { StandardCode = _certResponse.StandardCode }
                }).Build();

            _certificateLogs = Builder<CertificateLog>.CreateListOfSize(3).Build();

            _mockCertificateRepository = new Mock<ICertificateRepository>();
            _mockStandardRepository = new Mock<IStandardRepository>();
            _mockOrganisationQueryRepository = new Mock<IOrganisationQueryRepository>();

            _mockOrganisationQueryRepository.Setup(oqr => oqr.GetByUkPrn(_request.UkPrn)).ReturnsAsync(_organisationResponse);

            _mockStandardRepository.Setup(sr => sr.GetEpaoRegisteredStandards(_organisationResponse.EndPointAssessorOrganisationId, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_registeredStandards);

            _mockCertificateRepository.Setup(cr => cr.GetCertificate(_request.Uln, _request.StandardCode, _request.FamilyName))
                .ReturnsAsync(_certResponse);
            
            _mockCertificateRepository.Setup(cr => cr.GetCertificateLogsFor(_certResponse.Id))
                .ReturnsAsync(_certificateLogs as List<CertificateLog>);

            _handler = new GetBatchCertificateHandler(_mockCertificateRepository.Object,
                Mock.Of<IContactQueryRepository>(),
                _mockStandardRepository.Object,
                _mockOrganisationQueryRepository.Object,
                Mock.Of<ILogger<GetBatchCertificateHandler>>());
        }

        [Test]
        public async Task And_AllConditionsAreMet_Then_ReturnFullCertificate()
        {
            // Conditions that must be met - certificate not null, searchingOrganisation not null, status == draft or submitted
            // correct family name, LatestEpaOutcome = Pass, certificate org = searching org 

            var result = await _handler.Handle(_request, CancellationToken.None);

            result.Should().BeEquivalentTo(_certResponse, opt => opt.Excluding(c => c.UpdatedAt)
                .Excluding(c => c.UpdatedBy));
        }

        [Test]
        public async Task And_CertificateIsNull_Then_ReturnNull()
        {
            _mockCertificateRepository.Setup(cr => cr.GetCertificate(_request.Uln, _request.StandardCode, _request.FamilyName))
                .ReturnsAsync((Certificate)null);

            var result = await _handler.Handle(_request, CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task And_OrganisationIsNull_Then_ReturnNull()
        {
            _mockOrganisationQueryRepository.Setup(oqr => oqr.GetByUkPrn(_request.UkPrn))
                .ReturnsAsync((Organisation)null);

            var result = await _handler.Handle(_request, CancellationToken.None);

            result.Should().BeNull();
        }

        [TestCase("Ready")]
        [TestCase("Deleted")]
        public async Task And_StatusIsNotAllowed_Then_ReturnNull(string status)
        {
            _certResponse.Status = status;

            var result = await _handler.Handle(_request, CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task And_FamilyNameIsIncorrect_Then_ReturnNull()
        {
            _request.FamilyName = "Different name";

            var result = await _handler.Handle(_request, CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task And_OverallGradeIsFail_And_CertificateStatusIsSubmitted_Then_ReturnNull()
        {
            _certResponse.Status = CertificateStatus.Submitted;
            _certificateData.OverallGrade = CertificateGrade.Fail;
            RefreshCertificateResponse();

            var result = await _handler.Handle(_request, CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task And_OverallGradeIsNull_And_CertificateStatusIsDraft_And_LatestEpaOutcomeIsPass_Then_ReturnNull()
        {
            _certResponse.Status = CertificateStatus.Draft;
            _certificateData.OverallGrade = null;
            _certificateData.EpaDetails.LatestEpaOutcome = "Pass";
            RefreshCertificateResponse();

            var result = await _handler.Handle(_request, CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task And_LatestEpaOutcomeIsPass_And_SearchingOrganisationIsNotCertificateOrganisation_And_OrganisationIsNotApproved_Then_ReturnNull()
        {
            _organisationResponse.Id = Guid.NewGuid();

            _registeredStandards.PageOfResults = new List<EPORegisteredStandards>
            {
                Builder<EPORegisteredStandards>.CreateNew()
                .With(s => s.StandardCode = 2)
                .Build()
            };

            var result = await _handler.Handle(_request, CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task And_LatestEpaOutcomeIsPass_And_SearchingOrganisationIsNotCertificateOrganisation_And_OrganisationIsApprovedToAssess_Then_ReturnRedactedCertificate()
        {
            _organisationResponse.Id = Guid.NewGuid();

            var result = await _handler.Handle(_request, CancellationToken.None);

            VerifyRedactedCertificate(result);
        }

        private void RefreshCertificateResponse()
        {
            var certDataJson = JsonConvert.SerializeObject(_certificateData);

            _certResponse.CertificateData = certDataJson;
        }

        private void VerifyRedactedCertificate(Certificate result)
        {
            result.CertificateReference.Should().BeNull();
            result.CertificateReferenceId.Should().BeNull();
            result.CreateDay.Should().Be(DateTime.MinValue);
            result.CreatedAt.Should().Be(DateTime.MinValue);
            result.CreatedBy.Should().BeNull();
            result.UpdatedAt.Should().BeNull();
            result.UpdatedBy.Should().BeNull();
            result.DeletedBy.Should().BeNull();
            result.DeletedAt.Should().BeNull();

            var resultCertificateData = JsonConvert.DeserializeObject<CertificateData>(result.CertificateData);

            resultCertificateData.Should().BeEquivalentTo(_certificateData, opt => opt.Including(cd => cd.LearnerGivenNames)
                .Including(cd => cd.LearnerFamilyName)
                .Including(cd => cd.StandardReference)
                .Including(cd => cd.StandardName)
                .Including(cd => cd.StandardLevel)
                .Including(cd => cd.StandardPublicationDate));
        }
    }
}
