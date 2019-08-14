using SFA.DAS.AssessorService.Domain.Consts;
using System;
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
    }
}
