namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class UkPrnValidatorResult
    {
        public UkPrnValidatorResult(bool isValid)
        {
            IsValid = isValid;
        }

        public bool IsValid { get; private set; }
    }
}
