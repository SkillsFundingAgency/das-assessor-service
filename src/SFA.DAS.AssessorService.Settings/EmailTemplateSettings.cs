using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class EmailTemplateSettings : IEmailTemplateSettings
    {
        [JsonRequired]
        public string TemplateName { get; set; }
        [JsonRequired]
        public string TemplateId { get; set; }
        [JsonRequired]
        public string RecipientsAddress { get; set; }
        [JsonRequired]
        public string ReplyToAddress { get; set; }
        [JsonRequired]
        public string Subject { get; set; }
        [JsonRequired]
        public string SystemId { get; set; }
    }
}
