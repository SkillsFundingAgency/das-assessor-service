using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses
{
    internal static class EpaHelpers
    {
        public static string NormalizeEpaOutcome(string epaOutcome)
        {
            var outcomes = new string[] { EpaOutcome.Pass, EpaOutcome.Fail, EpaOutcome.Withdrawn };
            return outcomes.FirstOrDefault(g => g.Equals(epaOutcome, StringComparison.InvariantCultureIgnoreCase)) ?? epaOutcome;
        }

        public static Certificate ResetCertificateData(Certificate certificate)
        {
            // We need to clear out any old information (as it could be a deleted certificate, or the OverallGrade is no long correct)
            certificate.CertificateData =
                new CertificateData()
                {
                    LearnerGivenNames = certificate.CertificateData.LearnerGivenNames,
                    LearnerFamilyName = certificate.CertificateData.LearnerFamilyName,
                    LearningStartDate = certificate.CertificateData.LearningStartDate,
                    StandardReference = certificate.CertificateData.StandardReference,
                    Version = certificate.CertificateData.Version,
                    CourseOption = certificate.CertificateData.CourseOption,
                    StandardName = certificate.CertificateData.StandardName,
                    StandardLevel = certificate.CertificateData.StandardLevel,
                    StandardPublicationDate = certificate.CertificateData.StandardPublicationDate,
                    FullName = certificate.CertificateData.FullName,
                    ProviderName = certificate.CertificateData.ProviderName,
                    EpaDetails = new EpaDetails { EpaReference = certificate.CertificateReference, Epas = new List<EpaRecord>() }
                };

            return certificate;
        }
    }
}
