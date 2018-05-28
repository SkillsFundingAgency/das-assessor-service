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

        public List<CertificateResponse> Sanitise(List<CertificateResponse> certificateResponses)
        {
            var sanitizedCertificateResponses = new List<CertificateResponse>();

            foreach (var certificateResponse in certificateResponses)
            {
                var errorFlag = false;

                _aggregateLogger.LogInfo("Sanitising Certificates ...");

                var certificateData = certificateResponse.CertificateData;      
                if (string.IsNullOrEmpty(certificateData.ContactAddLine1))
                {
                   errorFlag = true;
                }

                if (string.IsNullOrEmpty(certificateData.ContactPostCode))
                {
                    errorFlag = true;                   
                }

                if (errorFlag)
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
