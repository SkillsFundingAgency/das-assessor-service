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

            _teardown = true;
               
          
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ConfigurationWrapper.GitUserName}:{ConfigurationWrapper.GitPassword}"));
            _webClient.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";


            if (_teardown)
            {
                repo.TearDownData();
            }
           

            try
            {
                using (var stream = new MemoryStream(_webClient.DownloadData(new Uri(ConfigurationWrapper.AssessmentOrgsUrl))))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var deliveryAreas = reader.HarvestDeliveryAreas(package);
                        repo.WriteDeliveryAreas(deliveryAreas);

                        var organisationTypes = reader.HarvestOrganisationTypes(package);
                        repo.WriteOrganisationTypes(organisationTypes);
                        var organisations = reader.HarvestEpaOrganisations(package, organisationTypes);
                        repo.WriteOrganisations(organisations);
                        var standards = reader.HarvestStandards(package);
                        repo.WriteStandards(standards);
                        var epaStandards = reader.HarvestEpaStandards(package, organisations, standards);
                        // import EPAStandards
                        // import StandardDeliveryAreas
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
