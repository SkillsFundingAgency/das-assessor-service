using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
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
        private readonly ILogger<StartFrameworkCertificateHandler> _logger;

        public StartFrameworkCertificateHandler(ICertificateRepository certificateRepository, ILogger<StartFrameworkCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<FrameworkCertificate> Handle(StartFrameworkCertificateRequest request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetFrameworkCertificate(request.FrameworkLearnerId);

            if (certificate == null)
            {
                certificate = await CreateNewFrameworkCertificate(request);
            }

            return certificate;
        }

        private async Task<FrameworkCertificate> CreateNewFrameworkCertificate(StartFrameworkCertificateRequest request)
        {
            _logger.LogDebug("Creating new framework certificate for: {FrameworkLearnerId}", request.FrameworkLearnerId);

            var certificate = new FrameworkCertificate
            {
                CertificateData = new CertificateData
                {
                    EpaDetails = new EpaDetails { Epas = new List<EpaRecord>() }
                },
                Status = CertificateStatus.Reprint,
                CreatedBy = request.Username,
                CertificateReference = string.Empty,
                FrameworkLearnerId = request.FrameworkLearnerId
            };

            var newCertificate = await _certificateRepository.NewFrameworkCertificate(await PopulateCertificateData(certificate, request));

            _logger.LogDebug("Created new framework certificate {CertificateReference} for: {FrameworkLearnerId}", certificate.CertificateReference, request.FrameworkLearnerId);

            return newCertificate;
        }

        /// <summary>
        /// This method can be used to create a new certificate where learner and provider information is populated
        /// or to populate an existing certificate when resuming the journey.
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="certData"></param>
        /// <param name="request"></param>
        /// <param name="organisation"></param>
        /// <returns></returns>
        private async Task<FrameworkCertificate> PopulateCertificateData(FrameworkCertificate certificate, StartFrameworkCertificateRequest request)
        {
            _logger.LogDebug("Populating certificate data for: {FrameworkLearnerId}", request.FrameworkLearnerId);

            /*
            var learner = await _learnerRepository.GetFrameworkLearner(request.FrameworkLearnerId);

            if ((learner.GivenNames.ToLower() == learner.GivenNames) || (learner.GivenNames.ToUpper() == learner.GivenNames))
            {
                certificate.CertificateData.LearnerGivenNames = _certificateNameCapitalisationService.ProperCase(learner.GivenNames);
            }
            else
            {
                certificate.CertificateData.LearnerGivenNames = learner.GivenNames;
            }

            if ((learner.FamilyName.ToLower() == learner.FamilyName) || (learner.FamilyName.ToUpper() == learner.FamilyName))
            {
                certificate.CertificateData.LearnerFamilyName = _certificateNameCapitalisationService.ProperCase(learner.FamilyName, true);
            }
            else
            {
                certificate.CertificateData.LearnerFamilyName = learner.FamilyName;
            }
            
            certificate.CertificateData.LearningStartDate = learner.LearnStartDate;
            certificate.CertificateData.FullName = $"{certificate.CertificateData.LearnerGivenNames} {certificate.CertificateData.LearnerFamilyName}";
            */

            // fake data for testing
            certificate.CertificateData.AchievementDate = DateTime.UtcNow;
            certificate.CertificateData.LearnerFamilyName = "Family";
            certificate.CertificateData.LearnerGivenNames = "Given";
            certificate.CertificateData.ContactAddLine1 = "AddLine1";
            certificate.CertificateData.ContactAddLine2 = "AddLine2";
            certificate.CertificateData.ContactAddLine3 = "AddLine3";
            certificate.CertificateData.ContactAddLine4 = "Addline4";
            certificate.CertificateData.ContactName = "ContactName";
            certificate.CertificateData.ContactPostCode = "Postcode";
            certificate.CertificateData.FullName = "Given Family";
            certificate.CertificateData.OverallGrade = CertificateGrade.Pass;
            certificate.CertificateData.LearningStartDate = DateTime.UtcNow;
            certificate.CertificateData.SendTo = CertificateSendTo.Apprentice;
            certificate.CertificateData.IncidentNumber = "INC1110010101";
            certificate.CertificateData.ReprintReasons = new List<string> { "Reasons" };
            
            return certificate;
        }
    }
}