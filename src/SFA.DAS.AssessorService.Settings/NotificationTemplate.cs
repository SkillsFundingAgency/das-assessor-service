using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Settings
{
    public class NotificationTemplate
    {
        /// The template name which is contained in the database
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// The template Id for the configurated GOV.UK Notify email template
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// The email addresses that the email template would be sent to
        /// </summary>
        public string Recipients { get; set; }
    }
}
