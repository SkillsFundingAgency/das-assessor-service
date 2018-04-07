using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;

//using Microsoft.Extensions.Configuration;
//using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Services;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow
{
    public class Command
    {
        private readonly IAggregateLogger _aggregateLogger;
        //    private readonly CoverLetterService _coverLetterService;
    //    private readonly IFACertificateService _ifaCertificateService;
    //    private readonly IConfiguration _configuration;

        //public Command(CoverLetterService coverLetterService,
        //    IFACertificateService ifaCertificateService,
        //    IConfiguration configuration)
        //{
        //    _coverLetterService = coverLetterService;
        //    _ifaCertificateService = ifaCertificateService;
        //    _configuration = configuration;
        //}

        public Command(IAggregateLogger aggregateLogger)
        {
            _aggregateLogger = aggregateLogger;
        }

        public async Task Execute()
        {
            _aggregateLogger.LogInfo("Print Function Flow Started");


            _aggregateLogger.LogInfo("101 Azure Function Demo - Accessing Environment variables");
            var customSetting = Environment.GetEnvironmentVariable("CustomSetting", EnvironmentVariableTarget.Process);
            _aggregateLogger.LogInfo($"Process Environment = {EnvironmentVariableTarget.Process}");



            //await _coverLetterService.Create();
            //await _ifaCertificateService.Create();

        }
    }
}
