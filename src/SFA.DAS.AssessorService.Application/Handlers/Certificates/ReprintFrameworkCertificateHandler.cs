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
    public class ReprintFrameworkCertificateHandler : IRequestHandler<ReprintFrameworkCertificateRequest, FrameworkCertificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IFrameworkLearnerRepository _frameworkLearnerRepository;
        private readonly ILogger<ReprintFrameworkCertificateHandler> _logger;

        public ReprintFrameworkCertificateHandler(ICertificateRepository certificateRepository, IFrameworkLearnerRepository frameworkLearnerRepository, 
            ILogger<ReprintFrameworkCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _frameworkLearnerRepository = frameworkLearnerRepository;
            _logger = logger;
        }

        public async Task<FrameworkCertificate> Handle(ReprintFrameworkCertificateRequest request, CancellationToken cancellationToken)
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
            else
            {
                certificate = await ReprintFrameworkCertificate(request, certificate);
            }
            
            return certificate;
        }

        private async Task<FrameworkCertificate> CreateNewFrameworkCertificate(ReprintFrameworkCertificateRequest request, FrameworkLearner frameworkLearner)
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
                    FrameworkCertificateNumber = frameworkLearner.FrameworkCertificateNumber,
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

        private async Task<FrameworkCertificate> ReprintFrameworkCertificate(ReprintFrameworkCertificateRequest request, FrameworkCertificate certificate)
        {
            _logger.LogDebug("Reprinting framework certificate for: {FrameworkLearnerId}", request.FrameworkLearnerId);

            certificate.Status = CertificateStatus.Reprint;
            certificate.CertificateData.ReprintReasons = request.Reasons.ToFlagsList();
            certificate.CertificateData.IncidentNumber = request.IncidentNumber;
            certificate.CertificateData.ContactName = request.ContactName;
            certificate.CertificateData.ContactAddLine1 = request.ContactAddLine1;
            certificate.CertificateData.ContactAddLine2 = request.ContactAddLine2;
            certificate.CertificateData.ContactAddLine3 = request.ContactAddLine3;
            certificate.CertificateData.ContactAddLine4 = request.ContactAddLine4;
            certificate.CertificateData.ContactPostCode = request.ContactPostcode;

            certificate = await _certificateRepository.UpdateFrameworkCertificate(certificate, request.Username, CertificateActions.Reprint);

            _logger.LogDebug("Reprinting framework certificate {CertificateReference} for: {FrameworkLearnerId}", certificate.CertificateReference, certificate.FrameworkLearnerId);

            return certificate;
        }
    }
}