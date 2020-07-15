using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class EmailTemplatesRecipients : BaseEntity
    {
        public Guid Id { get; set; }
        public string Recipients { get; set; }        
        public virtual EMailTemplate EMailTemplate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
    }
}
