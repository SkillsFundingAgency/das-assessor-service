namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IEpaOrganisationValidator
    {
        string CheckOrganisationId(string organisationId);
        string CheckOrganisationName(string name);
        string CheckIfOrganisationIdExists(string organisationId);
        string CheckIfOrganisationUkprnExists(long? ukprn);
        string CheckIfOrganisationUkprnExistsForOtherOrganisations(long? ukprn, string organisationIdToIgnore);
        string CheckOrganisationTypeIsNullOrExists(int? organisationTypeId);
        string CheckIfOrganisationNotFound(string organisationId);
        string CheckUkprnIsValid(long? ukprn);
        string CheckOrganisationNameNotUsed(string name);
        string CheckOrganisationNameNotUsedForOtherOrganisations(string name, string organisationIdToIgnore);
    }
}
