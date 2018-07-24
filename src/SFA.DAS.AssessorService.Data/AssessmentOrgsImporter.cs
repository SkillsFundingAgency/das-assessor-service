using System;
using System.IO;
using System.Net;
using System.Text;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Data
{
    public class AssessmentOrgsImporter : IAssessmentOrgsImporter
    {
        private readonly IAssessmentOrgsRepository _assessmentOrgsRepository;
        private static WebClient _webClient;
        private readonly IAssessmentOrgsSpreadsheetReader _spreadsheetReader;
        private readonly IWebConfiguration _configuration;
        private readonly ILogger<AssessmentOrgsImporter> _logger;
    
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

        public AssessmentOrgsImportResponse ImportAssessmentOrganisations(string action)
        {
            var progressStatus = new StringBuilder();
            bool buildup;

            switch (action.ToLower())
            {
                case "buildup":
                    progressStatus.Append($"BUILDUP instituted at [{DateTime.Now.ToLongTimeString()}]; ");
                    buildup = true;
                    break;        
                case "teardown":
                    progressStatus.Append($"TEARDOWN instituted at [{DateTime.Now.ToLongTimeString()}]; ");
                    buildup = false;
                    break;
                default:
                    progressStatus.Append($"NO ACTION instituted - action [{action.ToLower()}] not understood; ");
                    return new AssessmentOrgsImportResponse { Status = progressStatus.ToString() };
            }

            var spreadsheetDto = HarvestSpreadsheetData(progressStatus);
            TearDownDatabase(progressStatus);

            if (!buildup)
            {
                var message = $"Operations completed without build up  at [{DateTime.Now.ToLongTimeString()}]; ";
                _logger.LogInformation(message);
                progressStatus.Append(message);
                return new AssessmentOrgsImportResponse {Status = progressStatus.ToString()};
            }
      
            BuildUpDatabase(spreadsheetDto, progressStatus, action);
      
            return new AssessmentOrgsImportResponse {Status = progressStatus.ToString()};
        }

   
      
        private AssessmentOrganisationsSpreadsheetDto HarvestSpreadsheetData( StringBuilder progressStatus)
        {
            try
            {
                var spreadsheetDto = new AssessmentOrganisationsSpreadsheetDto();

                _webClient = new WebClient();
                var credentials =
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(
                            $"{_configuration.GitUsername}:{_configuration.GitPassword}"));
                _webClient.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";

                progressStatus.Append($"Downloading spreadsheet: [{_configuration.AssessmentOrgsUrl}]; ");

                using (var stream =
                    new MemoryStream(_webClient.DownloadData(new Uri(_configuration.AssessmentOrgsUrl))))
                {
                    progressStatus.Append("Opening spreadsheet as a stream; ");

                    using (var package = new ExcelPackage(stream))
                    {
                        progressStatus.Append("Reading from spreadsheet: Delivery Areas; ");
                        spreadsheetDto.DeliveryAreas = _spreadsheetReader.HarvestDeliveryAreas(package);
                        progressStatus.Append("Reading from spreadsheet: Organisation Types; ");
                        spreadsheetDto.OrganisationTypes = _spreadsheetReader.HarvestOrganisationTypes(package);
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

                        return spreadsheetDto;
                    }
                }
            }
            catch (Exception e)
            {
                progressStatus.Append("Error reading spreadsheet; ");
                _logger.LogError($"Progress details:  {progressStatus}", e);
                throw;
            }
        }

        private void TearDownDatabase(StringBuilder progressStatus)
        {
            _logger.LogInformation($"Teardown process started; ");
            var progress = _assessmentOrgsRepository.TearDownData();
            progressStatus.Append((string)progress);
            _logger.LogInformation(progress);
            _logger.LogInformation($"Teardown process stopped; ");
        }

        private void BuildUpDatabase(AssessmentOrganisationsSpreadsheetDto spreadsheetDto, StringBuilder progressStatus, string action)
        {
            try
            {
                var buildupStartMessage = $"BUILD UP [{action}] process started; ";
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
                _logger.LogInformation(message);
                progressStatus.Append(message);
                throw;
            }
        }
    }
}