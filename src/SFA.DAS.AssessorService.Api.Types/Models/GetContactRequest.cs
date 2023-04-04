using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetContactRequest: IRequest<AssessmentOrganisationContact>
    {
        private string contactId;

        public string ContactId
        {
            get { return contactId; }
            set { contactId = value; }
        }
        
    }
}