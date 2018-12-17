using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.Entities.AssessmentOrganisations;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Hosting;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class AssessmentOrgsImporter : IAssessmentOrgsImporter
    {
        private readonly IAssessmentOrgsRepository _assessmentOrgsRepository;
        private static WebClient _webClient;
        private readonly IAssessmentOrgsSpreadsheetReader _spreadsheetReader;
        private readonly IWebConfiguration _configuration;
        private readonly ILogger<AssessmentOrgsImporter> _logger;
        private readonly IHostingEnvironment _env;
        private readonly string TemplateFile = "assessmentOrgs.xlsx";

        public AssessmentOrgsImporter(IAssessmentOrgsRepository assessmentOrgsRepository,
            IAssessmentOrgsSpreadsheetReader spreadsheetReader,
            ILogger<AssessmentOrgsImporter> logger,
            IWebConfiguration configuration,
            IHostingEnvironment env)
        {
            _assessmentOrgsRepository = assessmentOrgsRepository;
            _spreadsheetReader = spreadsheetReader;
            _logger = logger;
            _configuration = configuration;
            _env = env;
        }

        public AssessmentOrgsImportResponse ImportAssessmentOrganisations()
        {
            if (!_env.IsDevelopment())
            {
                return new AssessmentOrgsImportResponse
                {
                    Status = "This patch will only run on development environments"
                };
            }

            var progressStatus = new StringBuilder();

            LogProgress(progressStatus, $"BUILDUP instituted at [{DateTime.Now.ToLongTimeString()}]; ");

            var spreadsheetDto = HarvestSpreadsheetData(progressStatus).Result;
            LogProgress(progressStatus, $"Spreadsheet harvested at [{DateTime.Now.ToLongTimeString()}]; ");

            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    TearDownDatabase(progressStatus);
                    BuildUpDatabase(spreadsheetDto, progressStatus);
                    progressStatus.Append("Running post build script; ");
                    _assessmentOrgsRepository.RunPostBuildScript();
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

            return new AssessmentOrgsImportResponse {Status = progressStatus.ToString()};
        }


        private async Task<AssessmentOrganisationsSpreadsheetDto> HarvestSpreadsheetData(StringBuilder progressStatus)
        {
            var spreadsheetDto = new AssessmentOrganisationsSpreadsheetDto();

            using (var memoryStream = new MemoryStream())
            {
                using (var fs = File.OpenRead($"..\\SFA.DAS.AssessorService.Database\\DataToImport\\{TemplateFile}"))
                {
                    fs.CopyTo(memoryStream);
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        LogProgress(progressStatus, "Reading from spreadsheet: Delivery Areas; ");
                        spreadsheetDto.DeliveryAreas = _spreadsheetReader.HarvestDeliveryAreas();
                        LogProgress(progressStatus, "Reading from spreadsheet: Organisation Types; ");
                        spreadsheetDto.OrganisationTypes = _spreadsheetReader.HarvestOrganisationTypes();
                        LogProgress(progressStatus, "Reading from spreadsheet: Organisations; ");
                        spreadsheetDto.Organisations =
                            _spreadsheetReader.HarvestEpaOrganisations(package, spreadsheetDto.OrganisationTypes);
                        LogProgress(progressStatus,
                            $"Reading from spreadsheet: Organisations gathered: {spreadsheetDto.Organisations?.Count}; ");
                        LogProgress(progressStatus, "Reading from spreadsheet: Standards; ");

                        LogProgress(progressStatus, "Reading from spreadsheet: Organisation-Standards; ");
                        spreadsheetDto.OrganisationStandards =
                            _spreadsheetReader.HarvestEpaOrganisationStandards(package, spreadsheetDto.Organisations);
                        LogProgress(progressStatus,
                            "Reading from spreadsheet: Organisation-Standards-Delivery Areas; ");
                        spreadsheetDto.OrganisationStandardDeliveryAreas =
                            _spreadsheetReader.HarvestStandardDeliveryAreas(package, spreadsheetDto.Organisations,
                                spreadsheetDto.DeliveryAreas);
                        LogProgress(progressStatus,
                            "Reading from spreadsheet: Organisation-Standards-Delivery Areas gathering comments; ");
                        _spreadsheetReader.MapDeliveryAreasCommentsIntoOrganisationStandards(
                            spreadsheetDto.OrganisationStandardDeliveryAreas, spreadsheetDto.OrganisationStandards);
                        LogProgress(progressStatus, "Reading from spreadsheet: Contacts; ");
                        spreadsheetDto.Contacts = _spreadsheetReader.HarvestOrganisationContacts(
                            spreadsheetDto.Organisations,
                            spreadsheetDto.OrganisationStandards);

                        _spreadsheetReader.MapPrimaryContacts(spreadsheetDto.Organisations, spreadsheetDto.Contacts);
                    }
                }
            }

            spreadsheetDto.Options = GetOptions();

            LogProgress(progressStatus, "Finished extracting from spreadsheet");
            return spreadsheetDto;

        }

        private void LogProgress(StringBuilder progressStatus, string status)
        {
            progressStatus.Append(status);
            _logger.LogInformation(status);
        }

        private void TearDownDatabase(StringBuilder progressStatus)
        {
            _logger.LogInformation($"Teardown process started; ");
            var progress = _assessmentOrgsRepository.TearDownData();
            progressStatus.Append(progress);
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


                var message = "WRITING TO DATABASE: Options; ";
                progressStatus.Append(message);
                _logger.LogInformation(message);
                _assessmentOrgsRepository.WriteOptions(spreadsheetDto.Options);

                message = "WRITING TO DATABASE: Delivery Areas; ";
                progressStatus.Append(message);
                _logger.LogInformation(message);
                _assessmentOrgsRepository.WriteDeliveryAreas(spreadsheetDto.DeliveryAreas);

                message = "WRITING TO DATABASE: Organisation Types; ";
                progressStatus.Append(message);
                _logger.LogInformation(message);
                _assessmentOrgsRepository.WriteOrganisationTypes(spreadsheetDto.OrganisationTypes);

                message = $"WRITING TO DATABASE: Organisations ({spreadsheetDto.Organisations?.Count}); ";
                progressStatus.Append(message);
                _logger.LogInformation(message);
                _assessmentOrgsRepository.WriteOrganisations(spreadsheetDto.Organisations);

                message = $"WRITING TO DATABASE: Contacts ({spreadsheetDto.Contacts?.Count});  ";
                progressStatus.Append(message);
                _logger.LogInformation(message);
                var contactsFromDatabase =
                    _assessmentOrgsRepository.UpsertThenGatherOrganisationContacts(spreadsheetDto.Contacts);

                var orgStandards = spreadsheetDto.OrganisationStandards;

                AttachContactsToOrganisationStandards(orgStandards, contactsFromDatabase);

                message = $"WRITING TO DATABASE: Organisation-Standards  ({orgStandards?.Count});";
                progressStatus.Append(message);
                _logger.LogInformation(message);
                var organisationStandards = _assessmentOrgsRepository.WriteEpaOrganisationStandards(orgStandards);

                message = "WRITING TO DATABASE: Organisation-Standard-Delivery Areas;  ";
                progressStatus.Append(message);
                _logger.LogInformation(message);
                _assessmentOrgsRepository.WriteStandardDeliveryAreas(spreadsheetDto.OrganisationStandardDeliveryAreas,
                    organisationStandards);

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

        private void AttachContactsToOrganisationStandards(List<EpaOrganisationStandard> orgStandards,
            List<OrganisationContact> contacts)
        {
            foreach (var standard in orgStandards)
            {
                var contact = contacts
                    .FirstOrDefault(x => x.EndPointAssessorOrganisationId == standard.EndPointAssessorOrganisationId &&
                                         x.Email != null && standard.ContactEmail != null && x.Email.Trim().ToLower() ==
                                         standard.ContactEmail.Trim().ToLower());

                if (contact != null)
                {
                    standard.ContactId = contact.Id;
                }

            }
        }

        private List<Domain.Entities.Option> GetOptions()
        {

            var options = new List<Option>();

            options.Add(new Option { StdCode = 272, OptionName = "Specialist role in Teaching" });
            options.Add(new Option { StdCode = 272, OptionName = "Specialist role in Research" });
            options.Add(new Option { StdCode = 240, OptionName = "Advanced Site Carpenter" });
            options.Add(new Option { StdCode = 240, OptionName = "Advanced Architectural Joiner" });
            options.Add(new Option { StdCode = 148, OptionName = "Credit risk" });
            options.Add(new Option { StdCode = 148, OptionName = "Advanced collections" });
            options.Add(new Option { StdCode = 148, OptionName = "Enforcement and recovery" });
            options.Add(new Option { StdCode = 114, OptionName = "Maintenance and rectification of aircraft avionic systems" });
            options.Add(new Option { StdCode = 114, OptionName = "Maintenance and rectification of power-plant (piston & turbine engines), propellers & rotors" });
            options.Add(new Option { StdCode = 114, OptionName = "Helicopter systems maintenance and practices" });
            options.Add(new Option { StdCode = 114, OptionName = "Fixed wing systems maintenance and practices" });
            options.Add(new Option { StdCode = 114, OptionName = "Application of business improvement techniques (personal accountability requirements) for working in an airworthiness environment (Maintenance practices)" });
            options.Add(new Option { StdCode = 114, OptionName = "Assembly, repair and replacement of pipe work for aircraft and engine systems" });
            options.Add(new Option { StdCode = 114, OptionName = "Inspect, repair, remove and replace aircraft electrical system wiring and components" });
            options.Add(new Option { StdCode = 114, OptionName = "Select and use appropriate electrical and avionic test equipment" });
            options.Add(new Option { StdCode = 114, OptionName = "Supervision of aircraft flight-line handling operations and/or second line maintenance activities" });
            options.Add(new Option { StdCode = 141, OptionName = "Work safely with others on Aircraft armament systems" });
            options.Add(new Option { StdCode = 141, OptionName = "Undertake Aircraft Assisted Escape System (AAES) checks and safety precautions associated with cockpit access" });
            options.Add(new Option { StdCode = 141, OptionName = "Act as safety person for another tradesperson undertaking a hazardous maintenance task" });
            options.Add(new Option { StdCode = 141, OptionName = "Undertake mechanical component replacement" });
            options.Add(new Option { StdCode = 141, OptionName = "Undertake avionic component replacement a solid grounding in the replacement of aircraft avionic components" });
            options.Add(new Option { StdCode = 141, OptionName = "Assist with Aircraft functional checks on non-complex systems" });
            options.Add(new Option { StdCode = 177, OptionName = "Craft" });
            options.Add(new Option { StdCode = 177, OptionName = "In store" });
            options.Add(new Option { StdCode = 177, OptionName = "Automated bakery" });
            options.Add(new Option { StdCode = 239, OptionName = "Site carpenter" });
            options.Add(new Option { StdCode = 239, OptionName = "Architectural joiner" });
            options.Add(new Option { StdCode = 50, OptionName = "Applied valuation and appraisal" });
            options.Add(new Option { StdCode = 50, OptionName = "Building pathology" });
            options.Add(new Option { StdCode = 50, OptionName = "Land, property and planning law" });
            options.Add(new Option { StdCode = 50, OptionName = "Procurement and contracts" });
            options.Add(new Option { StdCode = 50, OptionName = "Costing and cost planning of construction works" });
            options.Add(new Option { StdCode = 98, OptionName = "Technologist" });
            options.Add(new Option { StdCode = 98, OptionName = "Risk analyst" });
            options.Add(new Option { StdCode = 167, OptionName = "Aerospace Manufacturing Fitter" });
            options.Add(new Option { StdCode = 167, OptionName = "Aerospace Manufacturing Electrical/Mechanical and Systems Fitter" });
            options.Add(new Option { StdCode = 167, OptionName = "Aircraft Maintenance Fitter/ Technician (Fixed and Rotary Wing)" });
            options.Add(new Option { StdCode = 167, OptionName = "Airworthiness, Planning, Quality and Safety Technicial" });
            options.Add(new Option { StdCode = 167, OptionName = "Maritime Electrical Fitter" });
            options.Add(new Option { StdCode = 167, OptionName = "Maritime Mechanical Fitter" });
            options.Add(new Option { StdCode = 167, OptionName = "Maritime Fabricator" });
            options.Add(new Option { StdCode = 167, OptionName = "Maritime Pipeworker" });
            options.Add(new Option { StdCode = 167, OptionName = "Machinist – Advanced Manufacturing Engineering" });
            options.Add(new Option { StdCode = 167, OptionName = "Mechatronics Maintenance Technician" });
            options.Add(new Option { StdCode = 167, OptionName = "Product Design and Development Technician" });
            options.Add(new Option { StdCode = 167, OptionName = "Toolmaker and Tool and Die Maintenance Technician" });
            options.Add(new Option { StdCode = 167, OptionName = "Technical Support Technician" });
            options.Add(new Option { StdCode = 213, OptionName = "Retail banking" });
            options.Add(new Option { StdCode = 213, OptionName = "Commercial/Business banking" });
            options.Add(new Option { StdCode = 213, OptionName = "Investment banking" });
            options.Add(new Option { StdCode = 213, OptionName = "Investment management" });
            options.Add(new Option { StdCode = 213, OptionName = "Operations" });
            options.Add(new Option { StdCode = 213, OptionName = "Workplace pensions" });
            options.Add(new Option { StdCode = 126, OptionName = "Fire" });
            options.Add(new Option { StdCode = 126, OptionName = "Security" });
            options.Add(new Option { StdCode = 126, OptionName = "Fire and emergency lighting" });
            options.Add(new Option { StdCode = 126, OptionName = "Fire and security" });
            options.Add(new Option { StdCode = 245, OptionName = "Mechanical engineers" });
            options.Add(new Option { StdCode = 245, OptionName = "Automation engineers" });
            options.Add(new Option { StdCode = 245, OptionName = "Production engineers" });
            options.Add(new Option { StdCode = 182, OptionName = "Establishment and maintenance" });
            options.Add(new Option { StdCode = 182, OptionName = "Harvesting" });
            options.Add(new Option { StdCode = 137, OptionName = "General Furniture Manufacturer" });
            options.Add(new Option { StdCode = 137, OptionName = "Bed Manufacturer" });
            options.Add(new Option { StdCode = 137, OptionName = "Modern Upholsterer" });
            options.Add(new Option { StdCode = 137, OptionName = "Furniture Finisher" });
            options.Add(new Option { StdCode = 137, OptionName = "Fitted Furniture Installer" });
            options.Add(new Option { StdCode = 137, OptionName = "Furniture Restorer" });
            options.Add(new Option { StdCode = 137, OptionName = "Modern Furniture Service Repairer" });
            options.Add(new Option { StdCode = 137, OptionName = "Foam Convertor and Upholstery Cushion Interior Manufacturer" });
            options.Add(new Option { StdCode = 137, OptionName = "Wood Machinist" });
            options.Add(new Option { StdCode = 137, OptionName = "Furniture CNC Specialist" });
            options.Add(new Option { StdCode = 57, OptionName = "Network Maintenance Craftsperson (Electrical & Instrumentation)" });
            options.Add(new Option { StdCode = 57, OptionName = "Network Maintenance Craftsperson (Pressure Management)" });
            options.Add(new Option { StdCode = 57, OptionName = "Network Pipelines Maintenance Craftsperson" });
            options.Add(new Option { StdCode = 57, OptionName = "Emergency Response Craftsperson" });
            options.Add(new Option { StdCode = 254, OptionName = "Geospatial engineering" });
            options.Add(new Option { StdCode = 254, OptionName = "Hydrography" });
            options.Add(new Option { StdCode = 254, OptionName = "Utilities" });
            options.Add(new Option { StdCode = 254, OptionName = "Geospatial surveying" });
            options.Add(new Option { StdCode = 157, OptionName = "Hairdressing" });
            options.Add(new Option { StdCode = 157, OptionName = "Barbering" });
            options.Add(new Option { StdCode = 220, OptionName = "HSRI" });
            options.Add(new Option { StdCode = 220, OptionName = "Civil Engineering" });
            options.Add(new Option { StdCode = 220, OptionName = "Track" });
            options.Add(new Option { StdCode = 220, OptionName = "Systems Engineering" });
            options.Add(new Option { StdCode = 220, OptionName = "Command, Control and Communications" });
            options.Add(new Option { StdCode = 220, OptionName = "Rolling Stock" });
            options.Add(new Option { StdCode = 220, OptionName = "Power" });
            options.Add(new Option { StdCode = 220, OptionName = "Operations" });
            options.Add(new Option { StdCode = 181, OptionName = "Horticulture" });
            options.Add(new Option { StdCode = 181, OptionName = "Landscape construction" });
            options.Add(new Option { StdCode = 96, OptionName = "Food and beverage service" });
            options.Add(new Option { StdCode = 96, OptionName = "Alcholic beverage service" });
            options.Add(new Option { StdCode = 96, OptionName = "Barista" });
            options.Add(new Option { StdCode = 96, OptionName = "Food production" });
            options.Add(new Option { StdCode = 96, OptionName = "Concierge and guest services" });
            options.Add(new Option { StdCode = 96, OptionName = "Reservations" });
            options.Add(new Option { StdCode = 96, OptionName = "House-keeping" });
            options.Add(new Option { StdCode = 96, OptionName = "Reception" });
            options.Add(new Option { StdCode = 96, OptionName = "Conference and events operative" });
            options.Add(new Option { StdCode = 190, OptionName = "Core HR" });
            options.Add(new Option { StdCode = 190, OptionName = "Resourcing" });
            options.Add(new Option { StdCode = 190, OptionName = "Total reward" });
            options.Add(new Option { StdCode = 190, OptionName = "Organisation development" });
            options.Add(new Option { StdCode = 190, OptionName = "HR operations" });
            options.Add(new Option { StdCode = 44, OptionName = "Analytical Science" });
            options.Add(new Option { StdCode = 44, OptionName = "Chemical Science" });
            options.Add(new Option { StdCode = 44, OptionName = "Life Sciences" });
            options.Add(new Option { StdCode = 210, OptionName = "Finance Company" });
            options.Add(new Option { StdCode = 210, OptionName = "Retailer Consultant" });
            options.Add(new Option { StdCode = 246, OptionName = "Scheduler" });
            options.Add(new Option { StdCode = 246, OptionName = "Network Performance Operator" });
            options.Add(new Option { StdCode = 227, OptionName = "Nuclear Decommissioning Operative" });
            options.Add(new Option { StdCode = 227, OptionName = "Nuclear Process Operative" });
            options.Add(new Option { StdCode = 189, OptionName = "Bus" });
            options.Add(new Option { StdCode = 189, OptionName = "Coach" });
            options.Add(new Option { StdCode = 189, OptionName = "Rail" });
            options.Add(new Option { StdCode = 206, OptionName = "Station or depot" });
            options.Add(new Option { StdCode = 206, OptionName = "Onboard" });
            options.Add(new Option { StdCode = 225, OptionName = "Fossil Fuel – Natural Gas" });
            options.Add(new Option { StdCode = 225, OptionName = "Fossil Fuel - Oil" });
            options.Add(new Option { StdCode = 225, OptionName = "Fossil Fuel – Solid Fuel" });
            options.Add(new Option { StdCode = 225, OptionName = "Environmental Technologies" });
            options.Add(new Option { StdCode = 166, OptionName = "Research and Development Engineer" });
            options.Add(new Option { StdCode = 166, OptionName = "Design and Development Engineer" });
            options.Add(new Option { StdCode = 166, OptionName = "System Integration Engineer" });
            options.Add(new Option { StdCode = 166, OptionName = "Quality Assurance/Compliance Engineer" });
            options.Add(new Option { StdCode = 166, OptionName = "Test/Qualification Engineer" });
            options.Add(new Option { StdCode = 166, OptionName = "Manufacture/Production Engineer" });
            options.Add(new Option { StdCode = 166, OptionName = "Maintenance/Test Engineer" });
            options.Add(new Option { StdCode = 166, OptionName = "Product Support (including logistics) Engineer" });
            options.Add(new Option { StdCode = 166, OptionName = "Decommissioning/Disposal Engineer" });
            options.Add(new Option { StdCode = 166, OptionName = "Supply Chain/Procurement" });
            options.Add(new Option { StdCode = 166, OptionName = "Engineering Business Manager" });
            options.Add(new Option { StdCode = 284, OptionName = "Rearing" });
            options.Add(new Option { StdCode = 284, OptionName = "Breeding" });
            options.Add(new Option { StdCode = 284, OptionName = "Hatching" });
            options.Add(new Option { StdCode = 284, OptionName = "Egg production" });
            options.Add(new Option { StdCode = 284, OptionName = "Grow out" });
            options.Add(new Option { StdCode = 161, OptionName = "Power Transmission and Distribution Specific Engineering Knowledge" });
            options.Add(new Option { StdCode = 161, OptionName = "Electrical Engineering" });
            options.Add(new Option { StdCode = 161, OptionName = "Mechanical Engineering" });
            options.Add(new Option { StdCode = 161, OptionName = "Control & Instrumentation Engineering" });
            options.Add(new Option { StdCode = 117, OptionName = "Accounting" });
            options.Add(new Option { StdCode = 117, OptionName = "Tax" });
            options.Add(new Option { StdCode = 188, OptionName = "Signalling operator" });
            options.Add(new Option { StdCode = 188, OptionName = "Electrical controller" });
            options.Add(new Option { StdCode = 188, OptionName = "Incident responder" });
            options.Add(new Option { StdCode = 170, OptionName = "Adult" });
            options.Add(new Option { StdCode = 170, OptionName = "Children" });
            options.Add(new Option { StdCode = 170, OptionName = "Learning disability" });
            options.Add(new Option { StdCode = 170, OptionName = "Mental health" });
            options.Add(new Option { StdCode = 7, OptionName = "Corporate/Commercial" });
            options.Add(new Option { StdCode = 7, OptionName = "Retail" });
            options.Add(new Option { StdCode = 7, OptionName = "Wealth" });
            options.Add(new Option { StdCode = 7, OptionName = "Card services" });
            options.Add(new Option { StdCode = 144, OptionName = "Specialist risk" });
            options.Add(new Option { StdCode = 144, OptionName = "Financial crime" });
            options.Add(new Option { StdCode = 144, OptionName = "Compliance" });
            options.Add(new Option { StdCode = 144, OptionName = "Compliance / Risk (for smaller organisations)" });
            options.Add(new Option { StdCode = 31, OptionName = "Team leadership" });
            options.Add(new Option { StdCode = 31, OptionName = "N/A" }); ;
            options.Add(new Option { StdCode = 151, OptionName = "Adult nursing support" });
            options.Add(new Option { StdCode = 151, OptionName = "Maternity support" });
            options.Add(new Option { StdCode = 151, OptionName = "Theatre support" });
            options.Add(new Option { StdCode = 151, OptionName = "Mental health support" });
            options.Add(new Option { StdCode = 151, OptionName = "Children and young people support" });
            options.Add(new Option { StdCode = 151, OptionName = "Allied health profession – therapy support" });
            options.Add(new Option { StdCode = 270, OptionName = "Commercial/Business banking" });
            options.Add(new Option { StdCode = 270, OptionName = "Investment banking" });
            options.Add(new Option { StdCode = 270, OptionName = "Investment management" });
            options.Add(new Option { StdCode = 270, OptionName = "Investment operations" });
            options.Add(new Option { StdCode = 51, OptionName = "Valuation and appraisal" });
            options.Add(new Option { StdCode = 51, OptionName = "Building pathology" });
            options.Add(new Option { StdCode = 51, OptionName = "Land, property and planning law" });
            options.Add(new Option { StdCode = 51, OptionName = "Procurement and contracts" });
            options.Add(new Option { StdCode = 51, OptionName = "Costing and cost planning of construction works" });
            options.Add(new Option { StdCode = 32, OptionName = "Administrator" });
            options.Add(new Option { StdCode = 32, OptionName = "Consultant" });
            options.Add(new Option { StdCode = 152, OptionName = "Retail" });
            options.Add(new Option { StdCode = 152, OptionName = "Processing plant" });
            options.Add(new Option { StdCode = 152, OptionName = "In store" });
            options.Add(new Option { StdCode = 54, OptionName = "Retail" });
            options.Add(new Option { StdCode = 54, OptionName = "Process" });
            options.Add(new Option { StdCode = 16, OptionName = "Mechanical maintenance engineers" });
            options.Add(new Option { StdCode = 16, OptionName = "Multi-skilled maintenance engineers" });
            options.Add(new Option { StdCode = 58, OptionName = "Team leader service laying" });
            options.Add(new Option { StdCode = 58, OptionName = "Team leader main laying" });
            options.Add(new Option { StdCode = 6, OptionName = "Overhead lines" });
            options.Add(new Option { StdCode = 6, OptionName = "Underground cables" });
            options.Add(new Option { StdCode = 6, OptionName = "Substation fitting" });
            options.Add(new Option { StdCode = 53, OptionName = "Electrical" });
            options.Add(new Option { StdCode = 53, OptionName = "Mechnical" });
            options.Add(new Option { StdCode = 53, OptionName = "Instrumentation control and automation" });
            options.Add(new Option { StdCode = 27, OptionName = "Water treatment: Process technician" });
            options.Add(new Option { StdCode = 27, OptionName = "Water distribution: Network technician" });
            options.Add(new Option { StdCode = 27, OptionName = "Water distribution: Leakage technician" });
            options.Add(new Option { StdCode = 27, OptionName = "Waste water: Sewerage network technician" });
            options.Add(new Option { StdCode = 27, OptionName = "Waste water: Treatment technician" });
            options.Add(new Option { StdCode = 288, OptionName = "Solid plastering" });
            options.Add(new Option { StdCode = 288, OptionName = "Fibrous plastering" });
            options.Add(new Option { StdCode = 301, OptionName = "Rail civil engineer" });
            options.Add(new Option { StdCode = 301, OptionName = "Track engineering" });
            options.Add(new Option { StdCode = 301, OptionName = "Signalling and control systems" });
            options.Add(new Option { StdCode = 301, OptionName = "Rail systems integration" });
            options.Add(new Option { StdCode = 301, OptionName = "Traction and rolling stock" });
            options.Add(new Option { StdCode = 301, OptionName = "Telecoms, networks and digital" });
            options.Add(new Option { StdCode = 301, OptionName = "Electrical, mechanical or building services" });
            options.Add(new Option { StdCode = 88, OptionName = "Track advanced technician" });
            options.Add(new Option { StdCode = 88, OptionName = "Electrification advanced technician" });
            options.Add(new Option { StdCode = 88, OptionName = "Overhead lines advanced technician" });
            options.Add(new Option { StdCode = 88, OptionName = "Telecoms advanced technician" });
            options.Add(new Option { StdCode = 88, OptionName = "Signallng advanced technician" });
            options.Add(new Option { StdCode = 88, OptionName = "Traction & rolling stock advanced technician" });
            options.Add(new Option { StdCode = 88, OptionName = "Rail systems advanced technician" });
            options.Add(new Option { StdCode = 90, OptionName = "Track" });
            options.Add(new Option { StdCode = 90, OptionName = "Electrification " });
            options.Add(new Option { StdCode = 90, OptionName = "Overhead lines" });
            options.Add(new Option { StdCode = 90, OptionName = "Signalling" });
            options.Add(new Option { StdCode = 90, OptionName = "Telecoms" });
            options.Add(new Option { StdCode = 90, OptionName = "Traction & rolling stock (T&RS)" });
            options.Add(new Option { StdCode = 89, OptionName = "Track" });
            options.Add(new Option { StdCode = 89, OptionName = "Electrification " });
            options.Add(new Option { StdCode = 89, OptionName = "Overhead lines" });
            options.Add(new Option { StdCode = 89, OptionName = "Signalling" });
            options.Add(new Option { StdCode = 89, OptionName = "Telecoms" });
            options.Add(new Option { StdCode = 89, OptionName = "Traction & rolling stock" });
            options.Add(new Option { StdCode = 89, OptionName = "Rail systems" });
            options.Add(new Option { StdCode = 134, OptionName = "Mechanical" });
            options.Add(new Option { StdCode = 134, OptionName = "Electrical" });
            options.Add(new Option { StdCode = 134, OptionName = "Coach builder" });
            options.Add(new Option { StdCode = 134, OptionName = "Mechelec" });
            options.Add(new Option { StdCode = 314, OptionName = "Soil based system" });
            options.Add(new Option { StdCode = 314, OptionName = "Container based system" });
            options.Add(new Option { StdCode = 138, OptionName = "Food and beverage Supervisor" });
            options.Add(new Option { StdCode = 138, OptionName = "Bar supervisor" });
            options.Add(new Option { StdCode = 138, OptionName = "Housekeeping supervisor" });
            options.Add(new Option { StdCode = 138, OptionName = "Concierge supervisor" });
            options.Add(new Option { StdCode = 138, OptionName = "Front office supervisor" });
            options.Add(new Option { StdCode = 138, OptionName = "Events supervisor" });
            options.Add(new Option { StdCode = 138, OptionName = "Hospitality outlet supervisor" });
            options.Add(new Option { StdCode = 223, OptionName = "Food and Beverage manager" });
            options.Add(new Option { StdCode = 223, OptionName = "House keeping manager" });
            options.Add(new Option { StdCode = 223, OptionName = "Front office manager" });
            options.Add(new Option { StdCode = 223, OptionName = "Revenue manager" });
            options.Add(new Option { StdCode = 223, OptionName = "Conference and events manager" });
            options.Add(new Option { StdCode = 223, OptionName = "Hospitality outlet manager" });
            options.Add(new Option { StdCode = 223, OptionName = "Kitchen manager (head chef)" });
            options.Add(new Option { StdCode = 223, OptionName = "Multi-functional manager" });
            options.Add(new Option { StdCode = 25, OptionName = "Software Engineer" });
            options.Add(new Option { StdCode = 25, OptionName = "IT consultant" });
            options.Add(new Option { StdCode = 25, OptionName = "Business analyst" });
            options.Add(new Option { StdCode = 25, OptionName = "Cyber Security Specialist" });
            options.Add(new Option { StdCode = 25, OptionName = "Data analyst" });
            options.Add(new Option { StdCode = 25, OptionName = "Network Engineer" });
            return options;
        }
    }


}