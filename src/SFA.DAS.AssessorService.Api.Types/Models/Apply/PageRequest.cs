using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class PageRequest : IRequest<AssessorService.Api.Types.Models.Apply.Page>
    {
        public string UserId { get; }
        public string PageId { get; }

        public PageRequest(string userId, string pageId)
        {
            UserId = userId;
            PageId = pageId;
        }
    }
}