namespace SFA.DAS.AssessorService.Api.Types.Models.Azure
{
    public class CreateAzureUserSubscriptionRequest
    {
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; } = "My Subscription";
        public string State { get; set; } = "active";
    }
}
