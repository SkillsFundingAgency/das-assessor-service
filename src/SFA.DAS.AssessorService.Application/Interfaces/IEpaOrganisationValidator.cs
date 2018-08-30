namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IEpaOrganisationValidator
    {
        string CheckOrganisationIdIsPresentAndValid(string organisationId);
        string CheckOrganisationName(string name);
        string CheckIfOrganisationAlreadyExists(string organisationId);
        string CheckIfOrganisationUkprnExists(long? ukprn);
        string CheckIfOrganisationUkprnExistsForOtherOrganisations(long? ukprn, string organisationIdToIgnore);
        string CheckOrganisationTypeIsNullOrExists(int? organisationTypeId);
        string CheckIfOrganisationNotFound(string organisationId);
        string CheckUkprnIsValid(long? ukprn);
        string CheckIfStandardNotFound(int standardCode);
        string CheckIfOrganisationStandardAlreadyExists(string organisationId, int standardCode);

        string CheckIfOrganisationStandardDoesNotExist(string organisationId, int standardCode);
    }
}
