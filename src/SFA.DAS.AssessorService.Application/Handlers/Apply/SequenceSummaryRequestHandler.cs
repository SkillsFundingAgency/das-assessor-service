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
    public class SequenceSummaryRequestHandler : IRequestHandler<SequenceSummaryRequest, List<SequenceSummary>>
    {
        private readonly IDbConnection _connection;

        public SequenceSummaryRequestHandler(IDbConnection connection)
        {
            _connection = connection;
        }
        
        public async Task<List<SequenceSummary>> Handle(SequenceSummaryRequest request, CancellationToken cancellationToken)
        {
            var userWorkflow = await
                _connection.QuerySingleAsync<UserWorkflow>("SELECT * FROM UserWorkflows WHERE UserId = @UserId", request);

            var sequenceSummaries = JsonConvert.DeserializeObject<List<SequenceSummary>>(userWorkflow.Workflow);

            sequenceSummaries = sequenceSummaries.Where(ss => ss.Actor == "Applicant").ToList();
            
            return sequenceSummaries;
        }
    }
    
    public class AdminSequenceSummaryRequestHandler : IRequestHandler<AdminSequenceSummaryRequest, List<SequenceSummary>>
    {
        private readonly IDbConnection _connection;

        public AdminSequenceSummaryRequestHandler(IDbConnection connection)
        {
            _connection = connection;
        }
        
        public async Task<List<SequenceSummary>> Handle(AdminSequenceSummaryRequest request, CancellationToken cancellationToken)
        {
            var userWorkflow = await
                _connection.QuerySingleAsync<UserWorkflow>("SELECT * FROM UserWorkflows WHERE Id = @WorkflowId", request);

            var sequenceSummaries = JsonConvert.DeserializeObject<List<SequenceSummary>>(userWorkflow.Workflow);

            return sequenceSummaries;
        }
    }
}