using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using System;
using System.Linq;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Logger
{
    public class AggregateLogger : IAggregateLogger
    {
        private readonly ILogger _functionLogger;
        private readonly NLog.Logger _redisLogger;
        private readonly string _source;

        public AggregateLogger(string source, ILogger functionLogger, ExecutionContext executionContext)
        {
            _source = source;
            _functionLogger = functionLogger;

            var nLogFileName = GetNLogConfigurationFileName(source);

            LogManager.Configuration = new XmlLoggingConfiguration($@"{executionContext.FunctionAppDirectory}\{nLogFileName}.config");
            _redisLogger = LogManager.GetCurrentClassLogger();
        }

        public void LogError(string message, Exception ex)
        {
            _functionLogger.LogError(message, ex, _source);
            _redisLogger.Error(ex, message);
        }

        public void LogInfo(string message)
        {
            _functionLogger.LogInformation(message, _source);
            _redisLogger.Info(message);
        }

        private string GetNLogConfigurationFileName(string source)
        {
            var nLogFileName = "nlog." + source.Split('-').Last().Trim();
            return nLogFileName;
        }
    }
}