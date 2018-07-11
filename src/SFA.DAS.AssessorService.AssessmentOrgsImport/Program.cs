using System;
using System.IO;
using System.Net;
using System.Text;
using OfficeOpenXml;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport
{
     class Program
    {
        private static bool _teardown = false;
        private static WebClient _webClient;

        static void Main(string[] args)
        {
            var reader = new AssessmentOrganisationsReader();
            var repo = new AssessmentOrganisationsRepository();
            _webClient = new WebClient();
            
            _teardown = false;

            foreach (var arg in args)
            {
                if (arg == "-c")
                {
                    _teardown = true;
                }

                if (arg == "-h" || arg == "/h")
                {
                    Console.WriteLine("-c will tear down all the tables in the [ao] Schema.\nTHIS SHOULD ONLY BE USED AT FIRST DEPLOYMENT OR IF YOU WANT TO LOSE ALL DATA IN THOSE TABLES");
                    Console.Read();
                    return;
                }
            }
            
            if (_teardown)
            {
                repo.TearDownData();
            }

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ConfigurationWrapper.GitUserName}:{ConfigurationWrapper.GitPassword}"));
            _webClient.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";

            try
            {
                using (var stream = new MemoryStream(_webClient.DownloadData(new Uri(ConfigurationWrapper.AssessmentOrgsUrl))))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var statusCodes = repo.WriteStatusCodes();
                        var deliveryAreas = repo.WriteDeliveryAreas(reader.HarvestDeliveryAreas(package));
                        var organisationTypes = repo.WriteOrganisationTypes(reader.HarvestOrganisationTypes(package));
                        var organisations = repo.WriteOrganisations(reader.HarvestEpaOrganisations(package, organisationTypes, statusCodes));
                        var standards = repo.WriteStandards(reader.HarvestStandards(package, statusCodes));
                        repo.WriteEpaOrganisationStandards(reader.HarvestEpaOrganisationStandards(package, organisations, standards, statusCodes));
                        repo.WriteStandardDeliveryAreas(reader.HarvestStandardDeliveryAreas(package, organisations, standards, deliveryAreas));
                    }
                }
            }
            catch (Exception ex)
            {
                var z = ex.Message;
            }      
        }
    }
}
