namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public class Alert
    {
        public string Message { get; set; }
        public AlertType Type { get; set; }
    }

    public enum AlertType
    {
        Success,
        Info,
        Warning,
        Error
    }
}
