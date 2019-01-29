namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IValidationService
    {
        bool CheckEmailIsValid(string emailToCheck);
        bool IsNotEmpty(string stringToCheck);
        bool UkprnIsValid(string ukprn);
    }
}