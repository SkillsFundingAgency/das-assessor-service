using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IEpaOrganisationValidator
    {
        string CheckOrganisationId(string organisationId);
        string CheckOrganisationName(string name);

        Task<string> CheckIfOrganisationIdExists(string organisationId);
        Task<string> CheckIfOrganisationUkprnExists(long? ukprn);
    }
}
