using System;

namespace SFA.DAS.AssessorService.Domain.DTOs
{
    public class EmailTemplateSummary
    {
        public Guid Id { get; set; }
        public string TemplateName { get; set; }
        public string TemplateId { get; set; }
        public string Recipients { get; set; }
    }
}
