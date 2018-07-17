using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;

namespace SFA.DAS.AssessorService.EpaoImporter.services
{
    public class AssessmentOrgsImportService: IAssessmentOrgsImportService
    {

        private readonly IAssessmentOrganisationsRepository _repository;
        private readonly IAssessmentOrganisationsDataHandler _dataHandler;
        private WebClient _webClient;

        public AssessmentOrgsImportService(IAssessmentOrganisationsRepository repository, IAssessmentOrganisationsDataHandler dataHandler)
        {
            _repository = repository;
            _dataHandler = dataHandler;
        }


        public void ProcessAssessmentOrgs()
        {
            var webConfig = ConfigurationHelper.GetConfiguration();
            
            var gitPassword = webConfig.GitPassword;
            var gitUsername = webConfig.GitUsername;

            var credentials =
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
                        $"{gitUsername}:{gitPassword}"));
            _webClient.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";

            try

            {
                using (var stream =
                    new MemoryStream(_webClient.DownloadData(new Uri(webConfig.AssessmentOrgsUrl))))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var x = 1;
                        //var deliveryAreas = repo.WriteDeliveryAreas(dataHandler.HarvestDeliveryAreas(package));
                        //var organisationTypes =
                        //    repo.WriteOrganisationTypes(dataHandler.HarvestOrganisationTypes(package));
                        //var organisations =
                        //    repo.WriteOrganisations(
                        //        dataHandler.HarvestEpaOrganisations(package, organisationTypes));
                        //var standards = dataHandler.HarvestStandards(package);
                        //var organisationStandards =
                        //    dataHandler.HarvestEpaOrganisationStandards(package, organisations, standards);
                        //repo.WriteEpaOrganisationStandards(organisationStandards);
                        //repo.WriteStandardDeliveryAreas(
                        //    dataHandler.HarvestStandardDeliveryAreas(package, organisations, standards,
                        //        deliveryAreas));
                        //var contacts = dataHandler.GatherOrganisationContacts(organisations, organisationStandards);
                        //repo.WriteOrganisationContacts(contacts);
                    }
                }

                Console.WriteLine("Build up completed.");
            }
            catch (Exception ex)
            {
                var message = "Program stopped with exception message: " + ex.Message;
                Console.WriteLine(message);
                Console.Read();
            }
            return;
        }
    }
}
