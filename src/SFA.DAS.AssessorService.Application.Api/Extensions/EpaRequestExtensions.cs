using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Extensions
{
    public static class EpaRequestExtensions
    {
        public static string GetStandardId(this BatchEpaRequest request) => request.StandardCode > 0 ? request.StandardCode.ToString() : request.StandardReference;
        public static void PopulateMissingFields(this BatchEpaRequest request, Standard standard)
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

                if(string.IsNullOrWhiteSpace(request.Version))
                {
                    request.Version = standard.Version.VersionToString();
                }

                // StandardUId always populated, either from version supplied
                // Or Auto Calculated
                request.StandardUId = standard.StandardUId;
            }
        }
    }
}
