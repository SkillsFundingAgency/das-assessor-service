using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IEpaOrganisationValidator
    {
        string CheckOrganisationId(string organisationId);
        string CheckOrganisationName(string name);
        string CheckIfOrganisationIdExists(string organisationId);
        string CheckIfOrganisationUkprnExists(long? ukprn);
        string CheckOrganisationTypeIsNullOrExists(int? organisationTypeId);
    }
}
