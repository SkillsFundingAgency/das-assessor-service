using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class UpdatePageRequest : IRequest<Page>
    {
        public string UserId { get; }
        public string PageId { get; }
        public List<Question> Questions { get; }

        public UpdatePageRequest(string userId, string pageId, List<Question> questions)
        {
            UserId = userId;
            PageId = pageId;
            Questions = questions;
        }
    }
}