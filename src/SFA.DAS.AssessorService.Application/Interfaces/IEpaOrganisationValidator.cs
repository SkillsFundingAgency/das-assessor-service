using System.Collections.Generic;
using FluentValidation.Results;
using SFA.DAS.AssessorService.Api.Types.Models.Register;

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
        string CheckOrganisationNameNotUsed(string name);
        string CheckOrganisationNameNotUsedForOtherOrganisations(string name, string organisationIdToIgnore);

        string CheckIfContactIdIsEmptyOrValid(string contactId, string organisationId);
        string CheckIfOrganisationStandardDoesNotExist(string organisationId, int standardCode);

        ValidationResult ValidatorCreateEpaOrganisationRequest(CreateEpaOrganisationRequest request);
    }
}
