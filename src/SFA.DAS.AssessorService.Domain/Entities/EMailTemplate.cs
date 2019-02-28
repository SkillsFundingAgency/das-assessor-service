using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class EMailTemplate : BaseEntity                                                             
    {
        public Guid Id { get; set; }
        public string TemplateName { get; set; }
        public string TemplateId { get; set; }
        public string Recipients { get; set; }
        public string RecipientTemplate { get; set; }

    }
}
