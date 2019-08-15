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
            var certData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);

            // We need to clear out any old information (as it could be a deleted certificate, or the OverallGrade is no long correct)
            certificate.CertificateData = JsonConvert.SerializeObject(
                new CertificateData()
                {
                    LearnerGivenNames = certData.LearnerGivenNames,
                    LearnerFamilyName = certData.LearnerFamilyName,
                    LearningStartDate = certData.LearningStartDate,
                    StandardReference = certData.StandardReference,
                    StandardName = certData.StandardName,
                    StandardLevel = certData.StandardLevel,
                    StandardPublicationDate = certData.StandardPublicationDate,
                    FullName = certData.FullName,
                    ProviderName = certData.ProviderName,
                    EpaDetails = new EpaDetails { EpaReference = certificate.CertificateReference, Epas = new List<EpaRecord>() }
                });

            return certificate;
        }
    }
}
