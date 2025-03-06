using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Extensions;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class StartFrameworkCertificateHandler : IRequestHandler<StartFrameworkCertificateRequest, FrameworkCertificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IFrameworkLearnerRepository _frameworkLearnerRepository;
        private readonly ILogger<StartFrameworkCertificateHandler> _logger;

        public StartFrameworkCertificateHandler(ICertificateRepository certificateRepository, IFrameworkLearnerRepository frameworkLearnerRepository, 
            ILogger<StartFrameworkCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _frameworkLearnerRepository = frameworkLearnerRepository;
            _logger = logger;
        }

        public async Task<FrameworkCertificate> Handle(StartFrameworkCertificateRequest request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetFrameworkCertificate(request.FrameworkLearnerId);

            if (certificate == null)
            {
                var frameworkLearner = await _frameworkLearnerRepository.GetFrameworkLearner(request.FrameworkLearnerId);
                if (frameworkLearner != null)
                {
                    certificate = await CreateNewFrameworkCertificate(request, frameworkLearner);
                }
            }

            return certificate;
        }

        private async Task<FrameworkCertificate> CreateNewFrameworkCertificate(StartFrameworkCertificateRequest request, FrameworkLearner frameworkLearner)
        {
            _logger.LogDebug("Creating new framework certificate for: {FrameworkLearnerId}", request.FrameworkLearnerId);

            var certificate = new FrameworkCertificate
            {
                CertificateData = new CertificateData
                {
                    TrainingCode = frameworkLearner.TrainingCode,
                    LearnerGivenNames = frameworkLearner.ApprenticeForename,
                    LearnerFamilyName = frameworkLearner.ApprenticeSurname,
                    FullName = frameworkLearner.ApprenticeFullname,
                    LearningStartDate = frameworkLearner.ApprenticeStartdate,
                    AchievementDate = frameworkLearner.CertificationDate,
                    OverallGrade = CertificateGrade.Pass,
                    SendTo = CertificateSendTo.Apprentice,
                    PathwayName = frameworkLearner.PathwayName,
                    FrameworkName = frameworkLearner.FrameworkName,
                    FrameworkLevelName = frameworkLearner.ApprenticeshipLevelName,
                    ApprenticeReference = frameworkLearner.ApprenticeReference,
                    ReprintReasons = request.Reasons.ToFlagsList(),
                    IncidentNumber = request.IncidentNumber,
                    ContactName = request.ContactName,
                    ContactAddLine1 = request.ContactAddLine1,
                    ContactAddLine2 = request.ContactAddLine2,
                    ContactAddLine3 = request.ContactAddLine3,
                    ContactAddLine4 = request.ContactAddLine4,
                    ContactPostCode = request.ContactPostcode,
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord>() }
                },
                Status = CertificateStatus.Reprint,
                CreatedBy = request.Username,
                CertificateReference = string.Empty,
                FrameworkLearnerId = request.FrameworkLearnerId,
                ProviderUkPrn = int.TryParse(frameworkLearner.Ukprn, out int result ) ? result : null
            };

            certificate = await _certificateRepository.NewFrameworkCertificate(certificate);

            _logger.LogDebug("Created new framework certificate {CertificateReference} for: {FrameworkLearnerId}", certificate.CertificateReference, certificate.FrameworkLearnerId);

            return certificate;
        }
    }
}