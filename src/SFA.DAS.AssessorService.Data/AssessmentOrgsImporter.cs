using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Settings;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SFA.DAS.AssessorService.Data
{
    public class AssessmentOrgsImporter : IAssessmentOrgsImporter
    {
        private readonly IAssessmentOrgsRepository _assessmentOrgsRepository;
        private static WebClient _webClient;
        private readonly IAssessmentOrgsSpreadsheetReader _spreadsheetReader;
        private readonly IWebConfiguration _configuration;
        private readonly ILogger<AssessmentOrgsImporter> _logger;
        private readonly string TemplateFile = "assessmentOrgs.xlsx";

        public AssessmentOrgsImporter(IAssessmentOrgsRepository assessmentOrgsRepository,
                                        IAssessmentOrgsSpreadsheetReader spreadsheetReader,
                                        ILogger<AssessmentOrgsImporter> logger,
                                        IWebConfiguration configuration)
        {
            _assessmentOrgsRepository = assessmentOrgsRepository;
            _spreadsheetReader = spreadsheetReader;
            _logger = logger;
            _configuration = configuration;
        }

        public AssessmentOrgsImportResponse ImportAssessmentOrganisations()
        {
            var progressStatus = new StringBuilder();
           
            progressStatus.Append($"BUILDUP instituted at [{DateTime.Now.ToLongTimeString()}]; ");

            var spreadsheetDto = HarvestSpreadsheetData(progressStatus).Result;
            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    TearDownDatabase(progressStatus);
                    BuildUpDatabase(spreadsheetDto, progressStatus);
                    transactionScope.Complete();
                    progressStatus.Append("Entire Teardown/buildup transaction completed; ");
                }
                catch (Exception ex)
                {
                    transactionScope.Dispose();
                    var message = $"Error, transaction aborted: [{ex.Message}]; ";
                    _logger.LogError(message, ex);
                    progressStatus.Append(message);
                    throw;

                }
            }

            return new AssessmentOrgsImportResponse { Status = progressStatus.ToString() };
        }

        private async Task<AssessmentOrganisationsSpreadsheetDto> HarvestSpreadsheetData(StringBuilder progressStatus)
        {        
            var spreadsheetDto = new AssessmentOrganisationsSpreadsheetDto();

            var containerName = "assessmentorgs";
            var initialiseContainer = new BlobContainerHelper(_configuration);

            var container = initialiseContainer.GetContainer(containerName).Result;

            var blob = container.GetBlockBlobReference(TemplateFile);

            var memoryStream = new MemoryStream();
            await blob.DownloadToStreamAsync(memoryStream);

            using (var package = new ExcelPackage(memoryStream))
            {
                progressStatus.Append("Reading from spreadsheet: Delivery Areas; ");
                spreadsheetDto.DeliveryAreas = _spreadsheetReader.HarvestDeliveryAreas();
                progressStatus.Append("Reading from spreadsheet: Organisation Types; ");
                spreadsheetDto.OrganisationTypes = _spreadsheetReader.HarvestOrganisationTypes();
                progressStatus.Append("Reading from spreadsheet: Organisations; ");
                spreadsheetDto.Organisations =
                    _spreadsheetReader.HarvestEpaOrganisations(package, spreadsheetDto.OrganisationTypes);
                progressStatus.Append("Reading from spreadsheet: Standards; ");
                var standards = _spreadsheetReader.HarvestStandards(package);
                progressStatus.Append("Reading from spreadsheet: Organisation-Standards; ");
                spreadsheetDto.OrganisationStandards =
                    _spreadsheetReader.HarvestEpaOrganisationStandards(package, spreadsheetDto.Organisations,
                        standards);
                progressStatus.Append("Reading from spreadsheet: Organisation-Standards-Delivery Areas; ");
                spreadsheetDto.OrganisationStandardDeliveryAreas =
                    _spreadsheetReader.HarvestStandardDeliveryAreas(package, spreadsheetDto.Organisations, standards,
                        spreadsheetDto.DeliveryAreas);
                progressStatus.Append("Reading from spreadsheet: Contacts; ");
                spreadsheetDto.Contacts = _spreadsheetReader.HarvestOrganisationContacts(spreadsheetDto.Organisations,
                    spreadsheetDto.OrganisationStandards);
            }
            return spreadsheetDto;

        }

        private void TearDownDatabase(StringBuilder progressStatus)
        {
            _logger.LogInformation($"Teardown process started; ");
            var progress = _assessmentOrgsRepository.TearDownData();
            progressStatus.Append((string)progress);
            _logger.LogInformation(progress);
            _logger.LogInformation($"Teardown process stopped; ");
        }

        private void BuildUpDatabase(AssessmentOrganisationsSpreadsheetDto spreadsheetDto, StringBuilder progressStatus)
        {
            try
            {
                var buildupStartMessage = $"BUILD UP process started; ";
                _logger.LogInformation(buildupStartMessage);
                progressStatus.Append(buildupStartMessage);

                var message = "WRITING TO DATABASE: Delivery Areas; ";
                progressStatus.Append(message); _logger.LogInformation(message);
                _assessmentOrgsRepository.WriteDeliveryAreas(spreadsheetDto.DeliveryAreas);

                message = "WRITING TO DATABASE: Organisation Types; ";
                progressStatus.Append(message); _logger.LogInformation(message);
                _assessmentOrgsRepository.WriteOrganisationTypes(spreadsheetDto.OrganisationTypes);

                message = "WRITING TO DATABASE: Organisations; ";
                progressStatus.Append(message); _logger.LogInformation(message);
                _assessmentOrgsRepository.WriteOrganisations(spreadsheetDto.Organisations);

                message = "WRITING TO DATABASE: Organisation-Standards; ";
                progressStatus.Append(message); _logger.LogInformation(message);
                var organisationStandards = _assessmentOrgsRepository.WriteEpaOrganisationStandards(spreadsheetDto.OrganisationStandards);

                message = "WRITING TO DATABASE: Organisation-Standard-Delivery Areas;  ";
                progressStatus.Append(message); _logger.LogInformation(message);
                _assessmentOrgsRepository.WriteStandardDeliveryAreas(spreadsheetDto.OrganisationStandardDeliveryAreas, organisationStandards);

                message = "WRITING TO DATABASE: Contacts;  ";
                progressStatus.Append(message); _logger.LogInformation(message);
                _assessmentOrgsRepository.WriteOrganisationContacts(spreadsheetDto.Contacts);

                var buildupFinishedMessage = $"BUILD UP process completed  at [{DateTime.Now.ToLongTimeString()}]; ";
                _logger.LogInformation(buildupFinishedMessage);
                progressStatus.Append(buildupFinishedMessage);
            }
            catch (Exception ex)
            {
                var message = $"Program stopped with exception message: {ex.Message}; ";
                _logger.LogError(message, ex);
                progressStatus.Append(message);
                throw;
            }
        }
    }


    public class BlobContainerHelper
    {
        private readonly IWebConfiguration _webConfiguration;

        public BlobContainerHelper(IWebConfiguration webConfiguration)
        {
            _webConfiguration = webConfiguration;
        }
        public async Task<CloudBlobContainer> GetContainer(string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(_webConfiguration.IFATemplateStorageConnectionString);     
            var client = storageAccount.CreateCloudBlobClient();
            var blobContainer = client.GetContainerReference(containerName);

            var requestOptions = new BlobRequestOptions() { RetryPolicy = new NoRetry() };
            await blobContainer.CreateIfNotExistsAsync(requestOptions, null);
            return blobContainer;
        }
    }
}