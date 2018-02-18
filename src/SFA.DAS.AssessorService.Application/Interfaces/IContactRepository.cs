namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using System.Threading.Tasks;

    public interface IContactRepository
    {
        Task<bool> CheckContactExists(int contactId);
    }
}