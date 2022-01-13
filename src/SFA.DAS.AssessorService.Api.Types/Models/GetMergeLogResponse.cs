using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetMergeLogResponse
    {
        public IEnumerable<MergeOrganisation> MergeOrganisations { get; set; }
    }
}
