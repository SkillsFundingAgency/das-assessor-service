using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class UserWorkflow
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string Workflow { get; set; }
    }
}