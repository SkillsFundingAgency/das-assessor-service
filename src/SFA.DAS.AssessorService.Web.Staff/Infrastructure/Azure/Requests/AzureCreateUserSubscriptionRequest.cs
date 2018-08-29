namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure.Azure.Requests
{
    public class AzureCreateUserSubscriptionRequest
    {
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; } = "My Subscription";
        public string State { get; set; } = "active";
    }
}

