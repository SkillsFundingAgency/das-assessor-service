using System;
using System.IO;
using System.Net;
using System.Text;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading.Tasks;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Data.Configuration;

namespace SFA.DAS.AssessorService.Data
{
    public class AssessmentOrgsImporter : IAssessmentOrgsImporter
    {
        private readonly IAssessmentOrgsRepository _assessmentOrgsRepository;
        private static WebClient _webClient;
        private readonly IAssessmentOrgsSpreadsheetReader _spreadsheetReader;
        private readonly IConfigurationWrapper _configurationWrapper;


        public AssessmentOrgsImporter(IAssessmentOrgsRepository assessmentOrgsRepository, IConfigurationWrapper configurationWrapper, IAssessmentOrgsSpreadsheetReader spreadsheetReader)
        {
            _assessmentOrgsRepository = assessmentOrgsRepository;
            _configurationWrapper = configurationWrapper;
            _spreadsheetReader = spreadsheetReader;
        }

        public async Task<AssessmentOrgsImportResponse> ImportAssessmentOrganisations(string action)
        {
            var teardown = false;
            var buildup = false;

            switch (action.ToLower())
            {
                case "buildup":
                    teardown = true;
                    buildup = true;
                    break;
                case "teardown":
                    teardown = true;
                    break;
            }
            if (teardown)
            {
                // LOGGING
                _assessmentOrgsRepository.TearDownData();
                // LOGGING
            }

            _webClient = new WebClient();
            var credentials =
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
                        $"{_configurationWrapper.GitUserName}:{_configurationWrapper.GitPassword}"));
            _webClient.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";

            try

            {
                using (var stream =
                    new MemoryStream(_webClient.DownloadData(new Uri(_configurationWrapper.AssessmentOrgsUrl))))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var deliveryAreas = _assessmentOrgsRepository.WriteDeliveryAreas(_spreadsheetReader.HarvestDeliveryAreas(package));
                        var organisationTypes =
                            _assessmentOrgsRepository.WriteOrganisationTypes(_spreadsheetReader.HarvestOrganisationTypes(package));
                        var organisations =
                            _assessmentOrgsRepository.WriteOrganisations(
                                _spreadsheetReader.HarvestEpaOrganisations(package, organisationTypes));
                        var standards = _spreadsheetReader.HarvestStandards(package);
                        var organisationStandards =
                            _spreadsheetReader.HarvestEpaOrganisationStandards(package, organisations, standards);
                        _assessmentOrgsRepository.WriteEpaOrganisationStandards(organisationStandards);
                        _assessmentOrgsRepository.WriteStandardDeliveryAreas(
                            _spreadsheetReader.HarvestStandardDeliveryAreas(package, organisations, standards,
                                deliveryAreas));
                        var contacts = _spreadsheetReader.GatherOrganisationContacts(organisations, organisationStandards);
                        _assessmentOrgsRepository.WriteOrganisationContacts(contacts);
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
            return new AssessmentOrgsImportResponse {Status = action};
        }
    }
}