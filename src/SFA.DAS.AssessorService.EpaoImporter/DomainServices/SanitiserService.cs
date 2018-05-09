using System;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.EpaoImporter.Logger;

namespace SFA.DAS.AssessorService.EpaoImporter.DomainServices
{
    public class SanitiserService : ISanitiserService
    {
        private readonly IAggregateLogger _aggregateLogger;

        public SanitiserService(IAggregateLogger aggregateLogger)
        {
            _aggregateLogger = aggregateLogger;
        }

        public List<CertificateResponse> Sanitize(List<CertificateResponse> certificateResponses)
        {
            var sanitizedCertificateResponses = new List<CertificateResponse>();

            foreach (var certificateResponse in certificateResponses)
            {
                _aggregateLogger.LogInfo("Sanitizing Certificates ...");

                var certificateData = certificateResponse.CertificateData;

                var certificateContactDetails = "";

                if (!string.IsNullOrEmpty(certificateData.ContactAddLine1))
                {
                    certificateContactDetails += certificateData.ContactAddLine1 + System.Environment.NewLine;
                }

                if (!string.IsNullOrEmpty(certificateData.ContactPostCode))
                {
                    certificateContactDetails += certificateData.ContactPostCode + System.Environment.NewLine;
                }

                if (string.IsNullOrEmpty(certificateContactDetails))
                {
                    if (!string.IsNullOrEmpty(certificateData.LearnerGivenNames)
                        && !string.IsNullOrEmpty(certificateData.LearnerFamilyName))
                    {
                        _aggregateLogger.LogInfo(
                            $"Unprintable Certificate -> Given Names = {certificateData.LearnerGivenNames} Familly Name = {certificateData.LearnerFamilyName} - Cannot be processed");
                    }
                    else
                    {
                        _aggregateLogger.LogInfo($"Unprintable Certificate - Cannot be processed");
                    }
                }
                else
                {
                    sanitizedCertificateResponses.Add(certificateResponse);
                }
            }
            return sanitizedCertificateResponses;
        }
    }
}
