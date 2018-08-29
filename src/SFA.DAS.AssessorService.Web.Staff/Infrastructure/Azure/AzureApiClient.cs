using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure.Azure.Requests;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure.Azure.Responses;
using SFA.DAS.AssessorService.Web.Staff.Models.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public class AzureApiClient
    {
        private const int _pageSize = 5;
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;
        private readonly IAzureTokenService _tokenService;

        public AzureApiClient(HttpClient client, ILogger<ApiClient> logger, IAzureTokenService tokenService)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", _tokenService.CreateSharedAccessToken(DateTime.UtcNow.AddDays(1)));

            var res = await _client.GetAsync(new Uri(uri, UriKind.Relative));
            return await res.Content.ReadAsAsync<T>();
        }

        private async Task<U> Put<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", _tokenService.CreateSharedAccessToken(DateTime.UtcNow.AddDays(1)));

            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PutAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        private async Task<U> Post<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", _tokenService.CreateSharedAccessToken(DateTime.UtcNow.AddDays(1)));

            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }


        public async Task<PaginatedList<User>> ListUsers(int page)
        {    
            int skip = (page - 1) * _pageSize;

            var response = await Get<AzureUserResponse>($"/users?api-version=2017-03-01&expandGroups=false&$skip={skip}&$top={_pageSize}");
            return new PaginatedList<User>(response.Users, response.TotalCount, page, _pageSize);
        }

        public async Task<User> GetUserDetails(string userId, bool includeSubscriptions = false)
        {
            var user = await Get<User>($"/users/{userId}?api-version=2017-03-01");

            if (user.AzureId != null && includeSubscriptions)
            {
                var subscriptions = await GetSubscriptionsForUser(userId);
                user.Subscriptions.AddRange(subscriptions);
            }

            return user;
        }

        private async Task<List<Subscription>> GetSubscriptionsForUser(string userId)
        {
            var response = await Get<AzureSubscriptionResponse>($"/users/{userId}/subscriptions?api-version=2017-03-01");
            return response.Subscriptions;
        }

        public async Task<bool> UserExists(string email)
        {
            var response = await Get<AzureUserResponse>($"/users?api-version=2017-03-01&expandGroups=false&$top=1&$filter=email eq '{email}'");
            return response.Users.Any();
        }

        public async Task<User> CreateUser(string firstName, string lastName, string email, string ukprn, string productId)
        {
            var userId = Guid.NewGuid();
            var newUserRequest = new AzureCreateUserRequest { FirstName = firstName, LastName = lastName, Email = email, Note = ukprn };

            User user = await Put<AzureCreateUserRequest, User>($"/users/{userId}?api-version=2017-03-01", newUserRequest);

            if (user.AzureId != null)
            {
                Subscription subscription = await SubscribeUserToProduct(user.Id, productId);
                user.Subscriptions.Add(subscription);
            }

            return user;
        }

        private async Task<Subscription> SubscribeUserToProduct(string userId, string productId)
        {
            var subscriptionId = Guid.NewGuid();

            var newSubscriptionRequest = new AzureCreateUserSubscriptionRequest { UserId = $"/users/{userId}", ProductId = $"/products/{productId}" };
            return await Put<AzureCreateUserSubscriptionRequest, Subscription>($"/subscriptions/{subscriptionId}?api-version=2017-03-01", newSubscriptionRequest);
        }

        public async Task<List<Product>> ListProducts()
        {
            var response = await Get<AzureProductResponse>($"/products?api-version=2017-03-01");
            return response.Products;
        }

        public async Task<object> RegeneratePrimarySubscriptionKey(string subscriptionId)
        {
            return await Post<object, object>($"/subscriptions/{subscriptionId}/regeneratePrimaryKey?api-version=2017-03-01", null);
        }

        public async Task<object> RegenerateSecondarySubscriptionKey(string subscriptionId)
        {
            return await Post<object, object>($"/subscriptions/{subscriptionId}/regenerateSecondaryKey?api-version=2017-03-01", null);
        }
    }
}
