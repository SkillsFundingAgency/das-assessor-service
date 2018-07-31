using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using BoDi;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts;
using SFA.DAS.AssessorService.Settings;
using TechTalk.SpecFlow;
using Configuration = SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts.Configuration;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    [Binding]
    public class SpecflowIocRegister
    {
        private const string ServiceName = "SFA.DAS.AssessorService";
        private const string Version = "1.0";

        [BeforeTestRun]
        public static void RegisterTypes(IObjectContainer objectContainer)
        {
            var configurationConnecstionString =
                ConfigurationManager.AppSettings[Configuration.ConfigurationStorageConnectionString];                       
            var configuration = ConfigurationService.GetConfig("LOCAL", configurationConnecstionString, Version, ServiceName).Result;

            var sqlConnection = new SqlConnection(configuration.SpecflowDBTestConnectionString);
            objectContainer.RegisterInstanceAs<IDbConnection>(sqlConnection);
        }
    }
}