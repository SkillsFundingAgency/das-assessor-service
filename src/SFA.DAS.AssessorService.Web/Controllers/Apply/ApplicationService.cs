using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    public class ApplicationService : IApplicationService
    {
        private readonly IQnaApiClient _qnaApiClient;
        private readonly ILearnerDetailsApiClient _learnerDetailsApiClient;

        private const string WorkflowType = "EPAO";

        public ApplicationService(IQnaApiClient qnApiClient, ILearnerDetailsApiClient learnerDetailsApiClient)
        {
            _qnaApiClient = qnApiClient;
            _learnerDetailsApiClient = learnerDetailsApiClient;
        }

        public async Task<CreateApplicationRequest> BuildStandardWithdrawalCreateApplicationRequest(ContactResponse contact, OrganisationResponse organisation, int standardCode, string referenceFormat)
        {
            var pipelinesCount = await _learnerDetailsApiClient.GetPipelinesCount(organisation.EndPointAssessorOrganisationId, standardCode);
            
            var startApplicationRequest = new StartApplicationRequest
            {
                UserReference = contact.Id.ToString(),
                WorkflowType = WorkflowType,
                ApplicationData = JsonConvert.SerializeObject(new ApplicationData
                {
                    UseTradingName = false,
                    OrganisationName = organisation.EndPointAssessorName,
                    OrganisationReferenceId = organisation.Id.ToString(),
                    PipelinesCount = pipelinesCount
                })
            };

            return await BuildCreateApplicationRequest(startApplicationRequest, ApplicationTypes.StandardWithdrawal, contact.Id, organisation.Id, referenceFormat);
        }

        public async Task<CreateApplicationRequest> BuildOrganisationWithdrawalCreateApplicationRequest(ContactResponse contact, OrganisationResponse organisation, string referenceFormat)
        {
            var pipelinesCount = await _learnerDetailsApiClient.GetPipelinesCount(organisation.EndPointAssessorOrganisationId, null);

            var startApplicationRequest = new StartApplicationRequest
            {
                UserReference = contact.Id.ToString(),
                WorkflowType = WorkflowType,
                ApplicationData = JsonConvert.SerializeObject(new ApplicationData
                {
                    UseTradingName = false,
                    OrganisationName = organisation.EndPointAssessorName,
                    OrganisationReferenceId = organisation.Id.ToString(),
                    PipelinesCount = pipelinesCount
                })
            };

            return await BuildCreateApplicationRequest(startApplicationRequest, ApplicationTypes.OrganisationWithdrawal, contact.Id, organisation.Id, referenceFormat);
        }

        public async Task<CreateApplicationRequest> BuildCombinedCreateApplicationRequest(ContactResponse contact, OrganisationResponse organisation, string referenceFormat)
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

            return await BuildCreateApplicationRequest(startApplicationRequest, ApplicationTypes.Combined, contact.Id, organisation.Id, referenceFormat);
        }

        private async Task<CreateApplicationRequest> BuildCreateApplicationRequest(StartApplicationRequest startApplicationRequest, string applicationType, Guid contactId, Guid organisationId, string referenceFormat)
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
    }
}
