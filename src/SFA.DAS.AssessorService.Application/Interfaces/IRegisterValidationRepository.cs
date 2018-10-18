using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IRegisterValidationRepository
    {
        Task<bool> EpaOrganisationExistsWithOrganisationId(string organisationId);
        Task<bool> EpaOrganisationExistsWithUkprn(long ukprn);
        Task<bool> OrganisationTypeExists(int organisationTypeId);
        Task<bool> EpaOrganisationAlreadyUsingUkprn(long ukprn, string organisationId);
        Task<bool> EpaOrganisationAlreadyUsingName(string organisationName, string organisationId);
        Task<bool> ContactIdIsValid(string contactId);
        Task<bool> EmailAlreadyPresentInAnotherOrganisation(string email, string organisationId);
        Task<bool> ContactIdIsValidForOrganisationId(string contactId, string organisationId);
        Task<bool> EpaOrganisationStandardExists(string organisationId, int standardCode);
    }
}