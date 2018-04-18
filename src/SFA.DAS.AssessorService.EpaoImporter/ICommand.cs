using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public interface ICommand
    {
        Task Execute();
    }
}