using System;
using System.ComponentModel;

namespace SFA.DAS.AssessorService.Api.Types.Enums
{
    [Flags]
    public enum ReprintReasons
    {
        [Description("Delivery failed")]
        DeliveryFailed = 1,
        [Description("Apprentice moved to different employer or no longer with employer")]
        EmployerIncorrect = 2,
        [Description("Incorrect apprentice details")]
        ApprenticeDetails = 4,
        [Description("Incorrect apprentice address")]
        ApprenticeAddress = 8,
        [Description("Incorrect employer address")]
        EmployerAddress = 16,
        [Description("Incorrect apprenticeship details")]
        ApprenticeshipDetails = 32,
        [Description("Lost or damaged by receiver")]
        LostOrDamaged = 64
    }
}
