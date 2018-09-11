using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Exceptions;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class PageRequestHandler : IRequestHandler<PageRequest, Page>
    {
        private readonly IDbConnection _connection;

        public PageRequestHandler(IDbConnection connection)
        {
            _connection = connection;
        }
        
        public async Task<Page> Handle(PageRequest request, CancellationToken cancellationToken)
        {
            var userWorkflow = await
                _connection.QuerySingleAsync<UserWorkflow>("SELECT * FROM UserWorkflows WHERE UserId = @UserId", request);
            
            var workflow = JsonConvert.DeserializeObject<List<Sequence>>(userWorkflow.Workflow);

            var sequence = workflow.Single(w => w.Sections.Any(s => s.Pages.Any(p => p.PageId == request.PageId)));
            var section = sequence.Sections.Single(s => s.Pages.Any(p => p.PageId == request.PageId));

            if (!sequence.Active)
            {
                throw new UnauthorisedException("Sequence not active");
            }

            var page = section.Pages.Single(p => p.PageId == request.PageId);
            
            return page;
        }
    }
}