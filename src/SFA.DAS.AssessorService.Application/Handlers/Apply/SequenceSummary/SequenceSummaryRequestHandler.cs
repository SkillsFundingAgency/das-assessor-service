using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.SequenceSummary
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

            return sequenceSummaries;
        }
    }

    public class UserWorkflow
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string Workflow { get; set; }
    }
}