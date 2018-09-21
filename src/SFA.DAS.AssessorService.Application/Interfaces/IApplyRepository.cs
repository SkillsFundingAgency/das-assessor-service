using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    
    public interface IApplyRepository
    {
        Task<List<Sequence>> GetWorkflowDefinition();
        Task UpdateWorkflowDefinition(string definition);
        Task<List<Sequence>> GetSequences(string userId);
        Task<List<Sequence>> GetSequences(Guid id);
        Task UpdateUserWorkflow(List<Sequence> workflow, string userId);
    }
}