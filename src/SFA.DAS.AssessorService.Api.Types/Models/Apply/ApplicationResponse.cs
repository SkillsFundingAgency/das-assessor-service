using SFA.DAS.AssessorService.ApplyTypes;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Apply
{
    public class ApplicationResponse 
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string ApplicationStatus { get; set; }
        public ApplicationData ApplicationData { get; set; }
    }
}
