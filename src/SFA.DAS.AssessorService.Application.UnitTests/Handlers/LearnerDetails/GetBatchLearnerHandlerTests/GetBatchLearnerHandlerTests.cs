using AutoFixture.NUnit3;
using NUnit.Framework;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Learners;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using System.Threading;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.LearnerDetails.GetBatchLearnerHandlerTests
{
    public class GetBatchLearnerHandlerTests
    {

        [Test, RecursiveMoqAutoData]
        public async Task WhenCallingGetBatchLearnerHandler_WithoutRequestingCertificate_ReturnsLearnerInformation(
            [Frozen] Mock<IStandardService> mockStandardService,
            Standard standard,
            [Frozen] Mock<IIlrRepository> mockIlrRepo,
            Ilr learner,
            [Frozen] Mock<IOrganisationQueryRepository> mockOrgRepo,
            Organisation organisation,
            GetBatchLearnerRequest request,
            GetBatchLearnerHandler handler)
        {
            mockStandardService.Setup(s => s.GetStandardVersionById(request.Standard)).ReturnsAsync(standard);
            mockIlrRepo.Setup(s => s.Get(request.Uln, standard.LarsCode)).ReturnsAsync(learner);
            mockOrgRepo.Setup(s => s.GetByUkPrn(learner.UkPrn)).ReturnsAsync(organisation);

            var response = await handler.Handle(request, new CancellationToken());

            response.Certificate.Should().BeNull();
            var versionString = standard.Version.ToString();
            response.Learner.Should().BeEquivalentTo(new
            {
                learner.Uln,
                learner.GivenNames,
                learner.FamilyName,
                LearnerStartDate = learner.LearnStartDate,
                LearnerReferenceNumber = learner.LearnRefNumber,
                learner.PlannedEndDate,
                learner.CompletionStatus,
                EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                UkPrn = organisation.EndPointAssessorUkprn,
                OrganisationName = organisation.EndPointAssessorName,
                Standard = new
                {
                    standard.StandardUId,
                    standard.Title,
                    Version = versionString,
                    IFateReferenceNumber = standard.IfateReferenceNumber,
                    standard.LarsCode,
                    standard.Level,
                    standard.EffectiveFrom
                }
            });
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenCallingGetBatchLearnerHandler_RequestingCertificate_ReturnsCertificate(
            [Frozen] Mock<IStandardService> mockStandardService,
            Standard standard,
            [Frozen] Mock<IIlrRepository> mockIlrRepo,
            Ilr learner,
            [Frozen] Mock<IOrganisationQueryRepository> mockOrgRepo,
            Organisation organisation,
            [Frozen] Mock<IMediator> mockMediator,
            Certificate certificate,
            GetBatchLearnerRequest request,
            GetBatchLearnerHandler handler)
        {
            request.IncludeCertificate = true;
            mockStandardService.Setup(s => s.GetStandardVersionById(request.Standard)).ReturnsAsync(standard);
            mockIlrRepo.Setup(s => s.Get(request.Uln, standard.LarsCode)).ReturnsAsync(learner);
            mockOrgRepo.Setup(s => s.GetByUkPrn(learner.UkPrn)).ReturnsAsync(organisation);

            mockMediator.Setup(s => s.Send(It.Is<GetBatchCertificateRequest>(r =>
                r.Uln == request.Uln &&
                r.FamilyName == request.FamilyName &&
                r.StandardCode == standard.LarsCode &&
                r.StandardReference == standard.IfateReferenceNumber &&
                r.UkPrn == request.UkPrn), It.IsAny<CancellationToken>())).ReturnsAsync(certificate);

            var response = await handler.Handle(request, new CancellationToken());
            response.Certificate.Should().BeEquivalentTo(certificate);
        }
    }
}
