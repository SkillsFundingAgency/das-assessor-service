using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.QnA;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    public class ApplicationService : IApplicationService
    {
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IApplicationApiClient _applicationApiClient;
        private readonly ILearnerDetailsApiClient _learnerDetailsApiClient;
        private readonly IOrganisationsApiClient _organisationsApiClient;

        private const string WorkflowType = "EPAO";

        public ApplicationService(IQnaApiClient qnApiClient, IApplicationApiClient applicationApiClient, ILearnerDetailsApiClient learnerDetailsApiClient, IOrganisationsApiClient organisationsApiClient)
        {
            _qnaApiClient = qnApiClient;
            _applicationApiClient = applicationApiClient;
            _learnerDetailsApiClient = learnerDetailsApiClient;
            _organisationsApiClient = organisationsApiClient;
        }

        public async Task<CreateApplicationRequest> BuildStandardWithdrawalRequest(ContactResponse contact, OrganisationResponse organisation, int standardCode, string referenceFormat)
        {
            var pipelinesCount = _learnerDetailsApiClient.GetPipelinesCount(organisation.EndPointAssessorOrganisationId, standardCode);
            var earliestWithdrawalDate = _organisationsApiClient.GetEarliestWithdrawalDate(organisation.Id, standardCode);

            await Task.WhenAll(pipelinesCount, earliestWithdrawalDate);

            var startApplicationRequest = new StartApplicationRequest
            {
                UserReference = contact.Id.ToString(),
                WorkflowType = WorkflowType,
                ApplicationData = JsonConvert.SerializeObject(new ApplicationData
                {
                    UseTradingName = false,
                    OrganisationName = organisation.EndPointAssessorName,
                    OrganisationReferenceId = organisation.Id.ToString(),
                    PipelinesCount = pipelinesCount.Result,
                    EarliestDateOfWithdrawal = earliestWithdrawalDate.Result
                })
            };

            return await BuildRequest(startApplicationRequest, ApplicationTypes.StandardWithdrawal, contact.Id, organisation.Id, referenceFormat);
        }

        public async Task<CreateApplicationRequest> BuildOrganisationWithdrawalRequest(ContactResponse contact, OrganisationResponse organisation, string referenceFormat)
        {
            var pipelinesCount = _learnerDetailsApiClient.GetPipelinesCount(organisation.EndPointAssessorOrganisationId, null);
            var earliestWithdrawalDate = _organisationsApiClient.GetEarliestWithdrawalDate(organisation.Id, null);

            await Task.WhenAll(pipelinesCount, earliestWithdrawalDate);

            var startApplicationRequest = new StartApplicationRequest
            {
                UserReference = contact.Id.ToString(),
                WorkflowType = WorkflowType,
                ApplicationData = JsonConvert.SerializeObject(new ApplicationData
                {
                    UseTradingName = false,
                    OrganisationName = organisation.EndPointAssessorName,
                    OrganisationReferenceId = organisation.Id.ToString(),
                    PipelinesCount = pipelinesCount.Result,
                    EarliestDateOfWithdrawal = earliestWithdrawalDate.Result
                })
            };

            return await BuildRequest(startApplicationRequest, ApplicationTypes.OrganisationWithdrawal, contact.Id, organisation.Id, referenceFormat);
        }

        public async Task<CreateApplicationRequest> BuildInitialRequest(ContactResponse contact, OrganisationResponse organisation, string referenceFormat)
        {
            var startApplicationRequest = new StartApplicationRequest
            {
                UserReference = contact.Id.ToString(),
                WorkflowType = WorkflowType,
                ApplicationData = JsonConvert.SerializeObject(new ApplicationData
                {
                    UseTradingName = false,
                    OrganisationName = organisation.EndPointAssessorName,
                    OrganisationReferenceId = organisation.Id.ToString(),
                    OrganisationType = organisation.OrganisationType,
                    CompanySummary = organisation.CompanySummary,
                    CharitySummary = organisation.CharitySummary
                })
            };

            return await BuildRequest(startApplicationRequest, ApplicationTypes.Initial, contact.Id, organisation.Id, referenceFormat);
        }

        private async Task<CreateApplicationRequest> BuildRequest(StartApplicationRequest startApplicationRequest, string applicationType, Guid contactId, Guid organisationId, string referenceFormat)
        {
            var qnaResponse = await _qnaApiClient.StartApplications(startApplicationRequest);
            var sequences = await _qnaApiClient.GetAllApplicationSequences(qnaResponse.ApplicationId);
            var sections = sequences.Select(async sequence => await _qnaApiClient.GetSections(qnaResponse.ApplicationId, sequence.Id)).Select(t => t.Result).ToList();

            return new CreateApplicationRequest
            {
                ApplicationType = applicationType,
                QnaApplicationId = qnaResponse.ApplicationId,
                OrganisationId = organisationId,
                ApplicationReferenceFormat = referenceFormat,
                CreatingContactId = contactId,
                ApplySequences = sequences.Select(sequence => new ApplySequence
                {
                    SequenceId = sequence.Id,
                    Sections = sections.SelectMany(y => y.Where(x => x.SequenceNo == sequence.SequenceNo).Select(x => new ApplySection
                    {
                        SectionId = x.Id,
                        SectionNo = x.SectionNo,
                        Status = ApplicationSectionStatus.Draft,
                        RequestedFeedbackAnswered = x.QnAData.RequestedFeedbackAnswered
                    })).ToList(),
                    Status = ApplicationSequenceStatus.Draft,
                    IsActive = sequence.IsActive,
                    SequenceNo = sequence.SequenceNo,
                    NotRequired = sequence.NotRequired
                }).ToList()
            };
        }

        public async Task<bool> ResetApplicationToStage1(Guid id)
        {
            if (await _applicationApiClient.ResetApplicationToStage1(id))
            {
                var application = await _applicationApiClient.GetApplication(id);
                await _qnaApiClient.ResetSectionAnswers(application.ApplicationId, ApplyConst.STANDARD_SEQUENCE_NO, ApplyConst.STANDARD_DETAILS_SECTION_NO);
                return true;
            }

            return false;
        }
    }
}
