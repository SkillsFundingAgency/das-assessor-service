using System;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Logger
{
    public interface IAggregateLogger
    {
        void LogError(Exception ex, string message);
        void LogInformation(string message);
    }
}
