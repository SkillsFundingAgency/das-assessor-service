using System;

namespace SFA.DAS.AssessorService.Functions.Logger
{
    public interface IAggregateLogger
    {
        void LogError(string message, Exception ex);
        void LogInfo(string message);
    }
}