using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Data
{
    public class ApplyRepository : IApplyRepository
    {
        private readonly IDbConnection _dbConnection;

        public ApplyRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        
        public async Task<List<Sequence>> GetWorkflowDefinition()
        {
            var userWorkflow = await
                _dbConnection.QuerySingleAsync<WorkflowDefinition>("SELECT * FROM WorkflowDefinitions");

            return JsonConvert.DeserializeObject<List<Sequence>>(userWorkflow.Workflow);
        }

        public async Task UpdateWorkflowDefinition(string definition)
        {
            await _dbConnection.ExecuteAsync("UPDATE WorkflowDefinitions SET Workflow = @definition", new {definition});
        }

        public async Task<List<Sequence>> GetSequences(string userId)
        {
            var userWorkflow = await
                _dbConnection.QuerySingleAsync<UserWorkflow>("SELECT * FROM UserWorkflows WHERE UserId = @UserId",
                    new {userId});

            return JsonConvert.DeserializeObject<List<Sequence>>(userWorkflow.Workflow);
        }

        public async Task<List<Sequence>> GetSequences(Guid id)
        {
            var userWorkflow = await
                _dbConnection.QuerySingleAsync<UserWorkflow>("SELECT * FROM UserWorkflows WHERE Id = @id",
                    new {id});

            return JsonConvert.DeserializeObject<List<Sequence>>(userWorkflow.Workflow);
        }

        public async Task UpdateUserWorkflow(List<Sequence> workflow, string userId)
        {
            var workflowJson = JsonConvert.SerializeObject(workflow);

            await _dbConnection.ExecuteAsync("UPDATE UserWorkflows SET Workflow = @Workflow WHERE UserId = @UserId",
                new {userId, Workflow = workflowJson});
        }
    }

}