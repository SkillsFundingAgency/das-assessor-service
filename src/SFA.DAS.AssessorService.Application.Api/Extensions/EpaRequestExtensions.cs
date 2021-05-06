using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas;
using SFA.DAS.AssessorService.Domain.Entities;
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

                // only populate the standardUID if version was supplied
                if(!string.IsNullOrWhiteSpace(request.Version))
                {
                    request.StandardUId = standard.StandardUId;
                }
            }
        }
    }
}
