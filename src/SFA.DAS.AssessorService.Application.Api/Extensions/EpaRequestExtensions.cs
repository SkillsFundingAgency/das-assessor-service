using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Api.Extensions
{
    public static class EpaRequestExtensions
    {
        public static string GetStandardId(this BatchEpaRequest request) => !string.IsNullOrWhiteSpace(request.StandardReference) ? request.StandardReference : request.StandardCode > 0 ? request.StandardCode.ToString() : string.Empty;
        public static void PopulateMissingFields(this BatchEpaRequest request, Standard standard, Certificate existingCertificate = null)
        {
            if (standard != null)
            {
                // Only fill in the missing bits...
                if (request.StandardCode < 1)
                {
                    request.StandardCode = standard.LarsCode;
                }
                else if (string.IsNullOrEmpty(request.StandardReference))
                {
                    request.StandardReference = standard.IfateReferenceNumber;
                }

                if (string.IsNullOrWhiteSpace(request.Version))
                {
                    // As version not specified, version and standardUid are calculated, 
                    // but if existing certificate is not null, Prioritise those values
                    if (existingCertificate != null)
                    {
                        var certificateData = JsonConvert.DeserializeObject<CertificateData>(existingCertificate.CertificateData);
                        request.Version = certificateData.Version;
                        request.StandardUId = existingCertificate.StandardUId;
                    }

                    // If certificate was null, or the values were not there... override with calculated
                    if (string.IsNullOrWhiteSpace(request.Version))
                    {
                        // if version is null or empty, set version to the calculated version as a default.
                        request.Version = standard.Version;
                        request.StandardUId = standard.StandardUId;
                    }
                }
                else
                {
                    // If version isn't empty, we must have retrieved a specific version 
                    // subject to it being valid, so set just the StandardUId
                    request.StandardUId = standard.StandardUId;
                }
            }
        }
    }
}
