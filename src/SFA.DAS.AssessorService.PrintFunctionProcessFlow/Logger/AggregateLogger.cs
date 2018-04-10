using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using NLog;
using NLog.Config;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger
{
    public class AggregateLogger : IAggregateLogger
    {
        private readonly TraceWriter _functionLogger;
        private readonly global::NLog.Logger _redisLogger;
        private readonly ExecutionContext _executionContext;

        public AggregateLogger(TraceWriter functionLogger, ExecutionContext executionContext)
        {
            _functionLogger = functionLogger;
            _executionContext = executionContext;

            LogManager.Configuration = new XmlLoggingConfiguration($@"{executionContext.FunctionAppDirectory}\nlog.config");
            _redisLogger = LogManager.GetCurrentClassLogger();

        }

        public void LogError(string message, Exception ex)
        {
            _functionLogger.Error(message, ex);
            _redisLogger.Error(ex, message);
        }

        public void LogInfo(string message)
        {
            _functionLogger.Info(message);
            _redisLogger.Info(message);
        }
    }
}
