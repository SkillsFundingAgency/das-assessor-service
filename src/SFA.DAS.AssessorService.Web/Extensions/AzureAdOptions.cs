namespace Microsoft.AspNetCore.Authentication
{
    public class AzureAdOptions
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Instance { get; set; }

        public string Domain { get; set; }

        public string TenantId { get; set; }

        public string CallbackPath { get; set; }

        /// <summary>
        /// Authority delivering the token for your tenant
        /// </summary>
        public string Authority
        {
            get
            {
                return $"{Instance}{TenantId}";
            }
        }

        /// <summary>
        /// Client Id (Application ID) of the TodoListService, obtained from the Azure portal for that application
        /// </summary>
        public string ApiResourceId { get; set; }

        /// <summary>
        /// Base URL of the TodoListService
        /// </summary>
        public string ApiBaseAddress { get; set; }

        /// <summary>
        /// Instance of the settings for this Web application (to be used in controllers)
        /// </summary>
        public static AzureAdOptions Settings { set; get; }

        public string TokenEncodingKey { get; set; }
    }
}
