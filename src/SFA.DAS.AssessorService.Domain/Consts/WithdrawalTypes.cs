namespace SFA.DAS.AssessorService.Domain.Consts
{
    public class WithdrawalTypes
    {
        // Organisation is withdrawing from the register completely.
        public const string Register = "Register";

        // Organisation is withdrawing from a standard (any and all versions).
        public const string Standard = "Standard";

        // Organisation is withdrawing from one or more versions of a standard.
        public const string Version = "Version";
    }
}
