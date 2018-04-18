using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.EpaoImporter.Notification
{
    public interface INotificationService
    {
        Task Send();
    }
}