using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Functions
{
    public interface ICommand
    {
        Task Execute();
    }
}