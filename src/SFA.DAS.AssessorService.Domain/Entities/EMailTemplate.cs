using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class EMailTemplate
    {
        public Guid Id { get; set; }
        public string TemplateName { get; set; }
        public string TemplateId { get; set; }
    }
}
