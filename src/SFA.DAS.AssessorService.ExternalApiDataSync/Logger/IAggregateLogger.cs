using System;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Logger
{
    public interface IAggregateLogger
    {
        void LogError(string message, Exception ex);
        void LogInfo(string message);
    }
}
