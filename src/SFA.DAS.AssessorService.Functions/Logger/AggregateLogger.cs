using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using NLog;
using NLog.Config;

namespace SFA.DAS.AssessorService.Functions.Logger
{
    public class AggregateLogger : IAggregateLogger
    {
        private readonly TraceWriter _functionLogger;
        private readonly global::NLog.Logger _redisLogger;
        private readonly ExecutionContext _executionContext;
        private readonly string _source;

        public AggregateLogger(string source, TraceWriter functionLogger, ExecutionContext executionContext)
        {
            _source = source;
            _functionLogger = functionLogger;
            _executionContext = executionContext;

            var nLogFileName = GetNLogConfigurationFileName(source);

            LogManager.Configuration = new XmlLoggingConfiguration($@"{executionContext.FunctionAppDirectory}\{nLogFileName}.config");
            _redisLogger = LogManager.GetCurrentClassLogger();

        }

        public void LogError(string message, Exception ex)
        {
            _functionLogger.Error(message, ex, _source);
            _redisLogger.Error(ex, message);
        }

        public void LogInfo(string message)
        {
            _functionLogger.Info(message, _source);
            _redisLogger.Info(message);
        }

        private static string GetNLogConfigurationFileName(string source)
        {
            var nLogFileName = source.Split('-').Last().Trim() + ".nlog";
            return nLogFileName;
        }
    }
}
