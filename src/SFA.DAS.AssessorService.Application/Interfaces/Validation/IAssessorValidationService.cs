using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces.Validation
{
    public interface IAssessorValidationService
    {
        Task<bool> IsOrganisationNameTaken(string organisationName);
        Task<bool> IsOrganisationUkprnTaken(long ukprn);
        Task<bool> IsCompanyNumberTaken(string companyNumber);
        Task<bool> IsCharityNumberTaken(string charityNumber);

        Task<bool> IsEmailTaken(string email);
        Task<bool> CheckIfContactDetailsAlreadyPresentInSystem(string displayName, string email, string phone, Guid? contactId);

        Task<bool> IsOrganisationStandardTaken(string organisationId, int standardCode);
    }
}
