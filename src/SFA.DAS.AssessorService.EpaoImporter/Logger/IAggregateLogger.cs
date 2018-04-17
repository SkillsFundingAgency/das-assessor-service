using System;

namespace SFA.DAS.AssessorService.EpaoImporter.Logger
{
    public interface IAggregateLogger
    {
        void LogError(string message, Exception ex);
        void LogInfo(string message);
    }
}