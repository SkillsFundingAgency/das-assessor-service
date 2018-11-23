using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Azure
{
    public class AzureApiClient : AzureApiClientBase, IAzureApiClient
    {
        private readonly string _groupId;
        private readonly string _productId;
        protected readonly IOrganisationsApiClient _organisationsApiClient;
        protected readonly IContactsApiClient _contactsApiClient;

        public AzureApiClient(string baseUri, string productId, string groupId, IAzureTokenService tokenService, ILogger<AzureApiClientBase> logger, IOrganisationsApiClient organisationsApiClient, IContactsApiClient contactsApiClient) : base(baseUri, tokenService, logger)
        {
            _productId = productId;
            _groupId = groupId;
            _organisationsApiClient = organisationsApiClient;
            _contactsApiClient = contactsApiClient;
        }

        public async Task<PaginatedList<AzureUser>> ListUsers(int page)
        {
            int pageSize = 5;
            int skip = (page - 1) * pageSize;

            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/groups/{_groupId}/users?api-version=2017-03-01&$skip={skip}&$top={pageSize}"))
            {
                var response = await RequestAndDeserialiseAsync<AzureUserResponse>(httpRequest, "Could not get Users");
                return new PaginatedList<AzureUser>(response.Users.ToList(), response.TotalCount, page, pageSize);
            }
        }

        public async Task<AzureUser> GetUserDetails(string userId, bool includeSubscriptions = false, bool includeGroups = false)
        {
            AzureUser user;
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/users/{userId}?api-version=2017-03-01"))
            {
                user = await RequestAndDeserialiseAsync<AzureUser>(httpRequest, "Could not get User");
            }

            if (user.AzureId != null && includeSubscriptions)
            {
                var subscriptions = await GetSubscriptionsForUser(userId);
                user.Subscriptions.AddRange(subscriptions);
            }

            if (user.AzureId != null && includeGroups)
            {
                var groups = await GetGroupsForUser(userId);
                user.Groups.AddRange(groups);
            }

            return user;
        }

        public async Task<AzureUser> GetUserDetailsByUkprn(string ukprn, bool includeSubscriptions = false, bool includeGroups = false)
        {
            AzureUser user;
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/users?api-version=2017-03-01&expandGroups={includeGroups}&$top=1&$filter=note eq 'ukprn={ukprn}'"))
            {
                var response = await RequestAndDeserialiseAsync<AzureUserResponse>(httpRequest, "Could not get User");
                user = response.Users.FirstOrDefault();
            }

            if (user != null && user.AzureId != null && includeSubscriptions)
            {
                var subscriptions = await GetSubscriptionsForUser(user.Id);
                user.Subscriptions.AddRange(subscriptions);
            }

            return user;
        }

        public async Task<AzureUser> GetUserDetailsByEmail(string email, bool includeSubscriptions = false, bool includeGroups = false)
        {
            AzureUser user;
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/users?api-version=2017-03-01&expandGroups={includeGroups}&$top=1&$filter=email eq '{email}'"))
            {
                var response = await RequestAndDeserialiseAsync<AzureUserResponse>(httpRequest, "Could not get User");
                user = response.Users.FirstOrDefault();
            }

            if (user != null && user.AzureId != null && includeSubscriptions)
            {
                var subscriptions = await GetSubscriptionsForUser(user.Id);
                user.Subscriptions.AddRange(subscriptions);
            }

            return user;
        }

        private async Task<IEnumerable<AzureSubscription>> GetSubscriptionsForUser(string userId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/users/{userId}/subscriptions?api-version=2017-03-01"))
            {
                var response = await RequestAndDeserialiseAsync<AzureSubscriptionResponse>(httpRequest, "Could not get Subscriptions for User");

                foreach(var subscription in response.Subscriptions)
                {
                    var apiDetails = await GetApiDetailsForProduct(subscription.ProductId);
                    subscription.ServiceUrl = apiDetails?.ServiceUrl;
                }

                return response.Subscriptions;
            }            
        }

        private async Task<AzureApi> GetApiDetailsForProduct(string productId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/apis/{productId}?api-version=2017-03-01"))
            {
                return await RequestAndDeserialiseAsync<AzureApi>(httpRequest, "Could not get Api details for Product");
            }
        }

        private async Task<IEnumerable<AzureGroup>> GetGroupsForUser(string userId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/users/{userId}/groups?api-version=2017-03-01"))
            {
                var response = await RequestAndDeserialiseAsync<AzureGroupResponse>(httpRequest, "Could not get Groups for User");
                return response.Groups;
            }
        }

        public async Task<AzureUser> CreateUser(string ukprn, string username)
        {
            var organisation = await _organisationsApiClient.Get(ukprn);
            var contact = await _contactsApiClient.GetByUsername(username);

            AzureUser user = null;
            AzureUser ukprnUser = await GetUserDetailsByUkprn(ukprn, true, true);
            AzureUser emailUser = await GetUserDetailsByEmail(contact?.Email, true, true);

            if(ukprnUser != null && emailUser != null && ukprnUser.Id != emailUser.Id)
            {
                throw new Exceptions.EntityAlreadyExistsException($"Access is already enabled but not for the supplied ukprn and username.");
            }
            else if (ukprnUser != null)
            {
                user = ukprnUser;
            }
            else if(emailUser != null)
            {
                user = emailUser;
                await UpdateUserNote(user.Id, $"ukprn={ukprn}");
            }
            else
            {
                var request = new CreateAzureUserRequest
                {
                    FirstName = organisation.EndPointAssessorName,
                    LastName = contact.DisplayName,
                    Email = contact.Email,
                    Note = $"ukprn={ukprn}"
                };

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/users/{Guid.NewGuid()}?api-version=2017-03-01"))
                {
                    user = await PostPutRequestWithResponse<CreateAzureUserRequest, AzureUser>(httpRequest, request);
                }
            }

            if (user != null && !string.IsNullOrEmpty(user.Id))
            {
                if (!user.Groups.Any(g => string.Equals(g.Id, _groupId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    await AddUserToGroup(user.Id, _groupId);
                }

                if (!user.Groups.Any(g => string.Equals(g.Id, _productId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    await SubscribeUserToProduct(user.Id, _productId);
                }

                await _organisationsApiClient.Update(new UpdateOrganisationRequest
                {
                    EndPointAssessorName = organisation.EndPointAssessorName,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    EndPointAssessorUkprn = organisation.EndPointAssessorUkprn,
                    ApiEnabled = true,
                    ApiUser = user.Id.ToString()
                });

                // Note: Multiple things could have happened by this point so refresh
                user = await GetUserDetails(user.Id, true, true);
            }

            return user;
        }

        private async Task UpdateUserNote(string userId, string note)
        {
            var request = new UpdateAzureUserNoteRequest { Note = note };
            using (var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), $"/users/{userId}?api-version=2017-03-01"))
            {
                httpRequest.Headers.Add("If-Match", "*");
                await PostPutRequest(httpRequest, request);
            }
        }

        private async Task AddUserToGroup(string userId, string groupId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/groups/{groupId}/users/{userId}?api-version=2017-03-01"))
            {
                await PostPutRequest(httpRequest);
            }
        }

        private async Task RemoveUserFromGroup(string userId, string groupId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/groups/{groupId}/users/{userId}?api-version=2017-03-01"))
            {
                await PostPutRequest(httpRequest);
            }
        }

        private async Task<AzureSubscription> SubscribeUserToProduct(string userId, string productId)
        {
            var subscriptionId = Guid.NewGuid();
            var request = new CreateAzureUserSubscriptionRequest { UserId = $"/users/{userId}", ProductId = $"/products/{productId}" };

            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/subscriptions/{subscriptionId}?api-version=2017-03-01"))
            {     
                return await PostPutRequestWithResponse<CreateAzureUserSubscriptionRequest, AzureSubscription>(httpRequest, request);
            }
        }

        private async Task EnableSubscription(string subscriptionId)
        {
            var request = new EnableDisableAzureUserRequest { State = "active" };
            using (var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), $"/subscriptions/{subscriptionId}?api-version=2017-03-01"))
            {
                httpRequest.Headers.Add("If-Match", "*");
                await PostPutRequest(httpRequest, request);
            }
        }

        private async Task DisableSubscription(string subscriptionId)
        {
            var request = new EnableDisableAzureUserRequest { State = "suspended" };
            using (var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), $"/subscriptions/{subscriptionId}?api-version=2017-03-01"))
            {
                httpRequest.Headers.Add("If-Match", "*");
                await PostPutRequest(httpRequest, request);
            }
        }

        private async Task DeleteSubscription(string subscriptionId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/subscriptions/{subscriptionId}?api-version=2017-03-01"))
            {
                httpRequest.Headers.Add("If-Match", "*");
                await Delete(httpRequest);
            }
        }

        public async Task EnableUser(string userId)
        {
            var user = await GetUserDetails(userId);

            if (user != null)
            {
                var request = new EnableDisableAzureUserRequest { State = "active" };
                using (var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), $"/users/{userId}?api-version=2017-03-01"))
                {
                    httpRequest.Headers.Add("If-Match", "*");
                    await PostPutRequest(httpRequest, request);
                }
            }
            else
            {
                throw new Exceptions.EntityNotFoundException($"Could not find User");
            }
        }

        public async Task DisableUser(string userId)
        {
            var user = await GetUserDetails(userId);

            if (user != null)
            {
                var request = new EnableDisableAzureUserRequest { State = "blocked" };
                using (var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), $"/users/{userId}?api-version=2017-03-01"))
                {
                    httpRequest.Headers.Add("If-Match", "*");
                    await PostPutRequest(httpRequest, request);
                }
            }
            else
            {
                throw new Exceptions.EntityNotFoundException($"Could not find User");
            }
        }

        public async Task DeleteUser(string userId)
        {
            var user = await GetUserDetails(userId, true, true);

            if (user != null)
            {
                var apiGroups = user.Groups.Where(g => string.Equals(g.Id, "developers", StringComparison.InvariantCultureIgnoreCase) || string.Equals(g.Id, _groupId, StringComparison.InvariantCultureIgnoreCase));
                if (!user.Groups.Except(apiGroups).Any())
                {
                    // Doesn't belong to anything else; do an aggressive delete
                    using (var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/users/{userId}?api-version=2017-03-01&deleteSubscriptions=true"))
                    {
                        httpRequest.Headers.Add("If-Match", "*");
                        await Delete(httpRequest);
                    }
                }
                else
                {
                    // Only remove the appropriate subscriptions such that they can continue using other APIs
                    foreach (var subscription in user.Subscriptions.Where(s => string.Equals(s.ProductId, _productId, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        await DeleteSubscription(subscription.Id);
                    }

                    await RemoveUserFromGroup(user.Id, _groupId);
                    await UpdateUserNote(user.Id, string.Empty);
                }

                if (!string.IsNullOrWhiteSpace(user.Ukprn))
                {
                    var organisation = await _organisationsApiClient.Get(user.Ukprn);

                    if (organisation != null)
                    {
                        await _organisationsApiClient.Update(new UpdateOrganisationRequest
                        {
                            EndPointAssessorName = organisation.EndPointAssessorName,
                            EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                            EndPointAssessorUkprn = organisation.EndPointAssessorUkprn,
                            ApiEnabled = false,
                            ApiUser = null
                        });
                    }
                }
            }
        }

        public async Task<object> RegeneratePrimarySubscriptionKey(string subscriptionId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/subscriptions/{subscriptionId}/regeneratePrimaryKey?api-version=2017-03-01"))
            {
                await PostPutRequest(httpRequest);
                return null;
            }
        }

        public async Task<object> RegenerateSecondarySubscriptionKey(string subscriptionId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/subscriptions/{subscriptionId}/regenerateSecondaryKey?api-version=2017-03-01"))
            {
                await PostPutRequest(httpRequest);
                return null;
            }
        }
    }
}
