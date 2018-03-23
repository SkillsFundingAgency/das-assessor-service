using System;
using System.Configuration;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.TestSupport
{
    public class Configurator
    {
        private static Configurator configuratorInstance = null;

        private String browser;
        private String baseUrl;

        private Configurator()
        {
            browser = ConfigurationManager.AppSettings["Browser"];
            baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        }

        public static Configurator GetConfiguratorInstance()
        {
            if (configuratorInstance == null)
            {
                configuratorInstance = new Configurator();
            }
            return configuratorInstance;
        }

        public String GetBrowser()
        {
            return browser;
        }

        public String GetBaseUrl()
        {
            return baseUrl;
        }
    }
}