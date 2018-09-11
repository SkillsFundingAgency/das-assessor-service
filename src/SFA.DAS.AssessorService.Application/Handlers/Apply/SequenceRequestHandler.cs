using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class SequenceRequestHandler : IRequestHandler<SequenceRequest, Sequence>
    {
        private readonly IDbConnection _connection;

        public SequenceRequestHandler(IDbConnection connection)
        {
            _connection = connection;
        }
        
        public async Task<Sequence> Handle(SequenceRequest request, CancellationToken cancellationToken)
        {
            var userWorkflow = await
                _connection.QuerySingleAsync<UserWorkflow>("SELECT * FROM UserWorkflows WHERE UserId = @UserId", request);
            
            var workflow = JsonConvert.DeserializeObject<List<Sequence>>(userWorkflow.Workflow);

            return workflow.Single(w => w.SequenceId == request.SequenceId);
            //return sequenceSummaries;
        }
    }
    
    
}