using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class UpdatePageRequest : IRequest<UpdatePageResult>
    {
        public string UserId { get; }
        public string PageId { get; }
        public List<Answer> Answers { get; }

        public UpdatePageRequest(string userId, string pageId, List<Answer> answers)
        {
            UserId = userId;
            PageId = pageId;
            Answers = answers;
        }
    }
}