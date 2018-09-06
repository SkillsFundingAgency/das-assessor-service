namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IEpaOrganisationSearchValidator
    {
        bool IsValidEpaOrganisationId(string organisationIdToCheck);
        bool IsValidUkprn(string stringToCheck);
    }
}
