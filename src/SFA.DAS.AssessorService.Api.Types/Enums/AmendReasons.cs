using System;
using System.ComponentModel;

namespace SFA.DAS.AssessorService.Api.Types.Enums
{
    [Flags]
    public enum AmendReasons
    {
        [Description("Incorrect apprentice details")]
        ApprenticeDetails = 1,
        [Description("Incorrect apprentice address")]
        ApprenticeAddress = 2,
        [Description("Incorrect employer address")]
        EmployerAddress = 4,
        [Description("Incorrect apprenticeship details")]
        ApprenticeshipDetails = 8
    }
}
