using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.Extensions;

namespace SFA.DAS.AssessorService.Application.Api.Extensions
{
    public static class CertificateRequestExtensions
    {
        public static string GetStandardId(this BatchCertificateRequest request) => !string.IsNullOrWhiteSpace(request.StandardReference) ? request.StandardReference : request.StandardCode > 0 ? request.StandardCode.ToString() : string.Empty;
        public static void PopulateMissingFields(this BatchCertificateRequest request, Standard standard, Certificate existingCertificate = null)
        {
            if (standard != null)
            {
                if (request.StandardCode < 1)
                {
                    request.StandardCode = standard.LarsCode;
                }
                else if (string.IsNullOrEmpty(request.StandardReference))
                {
                    request.StandardReference = standard.IfateReferenceNumber;
                }

                if (string.IsNullOrWhiteSpace(request.CertificateData.Version))
                {
                    // As version not specified, version and standardUid are calculated, 
                    // but if existing certificate is not null, Prioritise those values
                    if (existingCertificate != null)
                    {
                        request.CertificateData.Version = existingCertificate.CertificateData.Version;
                        request.StandardUId = existingCertificate.StandardUId;
                    }

                    // If certificate was null, or the values were not there... override with calculated
                    if (string.IsNullOrWhiteSpace(request.CertificateData.Version))
                    {
                        // if version is null or empty, set version to the calculated version as a default.
                        request.CertificateData.Version = standard.Version;
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
