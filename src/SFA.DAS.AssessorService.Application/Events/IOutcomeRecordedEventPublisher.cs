using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Events
{
    public interface IOutcomeRecordedEventPublisher
    {
        /// <summary>
        /// Publishes OutcomeRecordedEvent when the update recorded a supported (pass/fail) outcome on a submitted certificate.
        /// </summary>
        Task PublishIfOutcomeRecorded(Certificate updated, string action);
    }
}
