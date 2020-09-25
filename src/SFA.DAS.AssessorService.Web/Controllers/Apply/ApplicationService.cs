using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    public class ApplicationService : IApplicationService
    {
        private readonly IQnaApiClient _qnaApiClient;
        
        private const string WorkflowType = "EPAO";

        public ApplicationService(IQnaApiClient qnApiClient)
        {
            _qnaApiClient = qnApiClient;
        }

        public async Task<CreateApplicationRequest> BuildCreateApplicationRequest(string applicationType, ContactResponse contact, OrganisationResponse org, string referenceFormat)
        {
            var startApplicationRequest = new StartApplicationRequest
            {
                UserReference = contact.Id.ToString(),
                WorkflowType = WorkflowType,
                ApplicationData = JsonConvert.SerializeObject(new ApplicationData
                {
                    UseTradingName = false,
                    OrganisationName = org.EndPointAssessorName,
                    OrganisationReferenceId = org.Id.ToString(),
                    OrganisationType = org.OrganisationType,
                    CompanySummary = org.CompanySummary,
                    CharitySummary = org.CharitySummary
                })
            };

            var qnaResponse = await _qnaApiClient.StartApplications(startApplicationRequest);
            var sequences = await _qnaApiClient.GetAllApplicationSequences(qnaResponse.ApplicationId);
            var sections = sequences.Select(async sequence => await _qnaApiClient.GetSections(qnaResponse.ApplicationId, sequence.Id)).Select(t => t.Result).ToList();

            return new CreateApplicationRequest
            {
                ApplicationType = applicationType,
                QnaApplicationId = qnaResponse.ApplicationId,
                OrganisationId = org.Id,
                ApplicationReferenceFormat = referenceFormat,
                CreatingContactId = contact.Id,
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

    public interface IApplicationService
    {
        Task<CreateApplicationRequest> BuildCreateApplicationRequest(string applicationType, ContactResponse contact, OrganisationResponse org, string referenceFormat);
    }
}
