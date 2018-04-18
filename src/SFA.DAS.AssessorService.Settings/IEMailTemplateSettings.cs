namespace SFA.DAS.AssessorService.Settings
{
    public interface IEmailTemplateSettings
    {
        string TemplateName { get; set; }
        string TemplateId { get; set; }
        string RecipientsAddress { get; set; }
        string ReplyToAddress { get; set; }
        string Subject { get; set; }
        string SystemId { get; set; }
    }
}
