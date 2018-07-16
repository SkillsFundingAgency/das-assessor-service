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
        private static bool _buildup = true;
        private static WebClient _webClient;
      
        static void Main(string[] args)
        {
            var dataHandler = new AssessmentOrganisationsDataHandler();
            var repo = new AssessmentOrganisationsRepository();
            _webClient = new WebClient();
            

            foreach (var arg in args)
            {
                if (arg.ToLower() == "-c")
                {
                    _teardown = true;
                }

                if (arg == "-C")
                {
                    _buildup = false;
                }

                if (arg == "-h" || arg == "/h")
                {
                    Console.WriteLine("-c will tear down all the tables that are exclusive to this import and selective delete records that appear to have been inserted by this process.\n" +
                                      "-C will teardown all tables and not rebuild them\n" +
                                      "\nTHIS SHOULD ONLY BE USED AT FIRST DEPLOYMENT OR IF YOU WANT TO LOSE ALL DATA IN THOSE TABLES");
                    Console.Read();
                    return;
                }
            }
            
            if (_teardown)
            {
                repo.TearDownData();
                Console.WriteLine("Teardown complete.");
            }

            if (_buildup == false)
            {
                Console.WriteLine("Program completed, no buildup required.");
                Console.Read();
                return;
            }

            var credentials =
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
                        $"{ConfigurationWrapper.GitUserName}:{ConfigurationWrapper.GitPassword}"));
            _webClient.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";

            try

            {
                using (var stream =
                    new MemoryStream(_webClient.DownloadData(new Uri(ConfigurationWrapper.AssessmentOrgsUrl))))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var deliveryAreas = repo.WriteDeliveryAreas(dataHandler.HarvestDeliveryAreas(package));
                        var organisationTypes =
                            repo.WriteOrganisationTypes(dataHandler.HarvestOrganisationTypes(package));
                        var organisations =
                            repo.WriteOrganisations(
                                dataHandler.HarvestEpaOrganisations(package, organisationTypes));
                        var standards = dataHandler.HarvestStandards(package);
                        var organisationStandards =
                            dataHandler.HarvestEpaOrganisationStandards(package, organisations, standards);
                        repo.WriteEpaOrganisationStandards(organisationStandards);
                        repo.WriteStandardDeliveryAreas(
                            dataHandler.HarvestStandardDeliveryAreas(package, organisations, standards,
                                deliveryAreas));
                        var contacts = dataHandler.GatherOrganisationContacts(organisations, organisationStandards);
                        repo.WriteOrganisationContacts(contacts);
                    }
                }

                Console.WriteLine("Build up completed.");
            }
            catch (Exception ex)
            {
                var message = "Program stopped with exception message: "+ ex.Message;
                Console.WriteLine(message);
                Console.Read();
            }
        }
    }
}
