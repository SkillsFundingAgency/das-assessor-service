using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Page
{
    public class PageRequest : IRequest<Sequence.Page>
    {
        public int UserId { get; }
        public string PageId { get; }

        public PageRequest(int userId, string pageId)
        {
            UserId = userId;
            PageId = pageId;
        }
    }
}