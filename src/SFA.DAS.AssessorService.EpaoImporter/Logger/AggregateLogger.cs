using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
namespace SFA.DAS.AssessorService.EpaoImporter.Logger
{
    public class AggregateLogger : IAggregateLogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger _functionLogger;
        private readonly global::NLog.Logger _redisLogger;
        private readonly string _source;

        public AggregateLogger(string source, Microsoft.Extensions.Logging.ILogger functionLogger, ExecutionContext executionContext)
        {
            _source = source;
            _functionLogger = functionLogger;

            var nLogFileName = GetNLogConfigurationFileName(source);

            LogManager.Configuration = new XmlLoggingConfiguration($@"{executionContext.FunctionAppDirectory}\{nLogFileName}.config");
            _redisLogger = LogManager.GetCurrentClassLogger();

        }

        public void LogError(string message, Exception ex)
        {
            _functionLogger.LogError(ex, message);
            _redisLogger.Error(ex, message);
        }

        public void LogInfo(string message)
        {
            var logMessage = $"Source: {_source} - Message: {message}";
            _functionLogger.LogInformation(logMessage);
            _redisLogger.Info(message);
        }
        
        private string GetNLogConfigurationFileName(string source)
        {
            var nLogFileName = "nlog." + source.Split('-').Last().Trim();
            return nLogFileName;
        }
    }
}
