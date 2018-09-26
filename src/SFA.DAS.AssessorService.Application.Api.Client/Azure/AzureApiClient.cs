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
        private readonly string _productId;
        protected readonly IOrganisationsApiClient _organisationsApiClient;
        protected readonly IContactsApiClient _contactsApiClient;

        public AzureApiClient(string baseUri, string productId, IAzureTokenService tokenService, ILogger<AzureApiClientBase> logger, IOrganisationsApiClient organisationsApiClient, IContactsApiClient contactsApiClient) : base(baseUri, tokenService, logger)
        {
            _productId = productId;
            _organisationsApiClient = organisationsApiClient;
            _contactsApiClient = contactsApiClient;
        }

        public async Task<PaginatedList<AzureUser>> ListUsers(int page)
        {
            int pageSize = 5;
            int skip = (page - 1) * pageSize;

            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/users?api-version=2017-03-01&expandGroups=false&$skip={skip}&$top={pageSize}"))
            {
                var response = await RequestAndDeserialiseAsync<AzureUserResponse>(httpRequest, "Could not get Users");
                return new PaginatedList<AzureUser>(response.Users.ToList(), response.TotalCount, page, pageSize);
            }
        }

        public async Task<AzureUser> GetUserDetails(string userId, bool includeSubscriptions = false)
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

            return user;
        }

        public async Task<AzureUser> GetUserDetailsByUkprn(string ukprn, bool includeSubscriptions = false)
        {
            AzureUser user;
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/users?api-version=2017-03-01&expandGroups=false&$top=1&$filter=note eq 'ukprn={ukprn}'"))
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

        public async Task<AzureUser> GetUserDetailsByEmail(string email, bool includeSubscriptions = false)
        {
            AzureUser user;
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/users?api-version=2017-03-01&expandGroups=false&$top=1&$filter=email eq '{email}'"))
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
                return response.Subscriptions;
            }            
        }

        public async Task<AzureUser> CreateUser(string ukprn, string username)
        {
            var userId = Guid.NewGuid();
            var organisation = await _organisationsApiClient.Get(ukprn);
            var contact = await _contactsApiClient.GetByUsername(username);

            if (await GetUserDetailsByUkprn(ukprn) != null) throw new Exceptions.EntityAlreadyExistsException($"Access is already enabled for ukprn {ukprn}");
            else if (await GetUserDetailsByEmail(contact.Email) != null) throw new Exceptions.EntityAlreadyExistsException($"Access is already enabled for username {username}");

            var request = new CreateAzureUserRequest
            {
                FirstName = organisation.EndPointAssessorName,
                LastName = "EPAO",
                Email = contact.Email,
                Note = $"ukprn={ukprn}",
                Password = "3pa0Pa55w0rd!"
            };

            AzureUser user;
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/users/{userId}?api-version=2017-03-01"))
            {
                user = await PostPutRequestWithResponse<CreateAzureUserRequest, AzureUser>(httpRequest, request);
            }

            if (user.AzureId != null)
            {
                var subscription = await SubscribeUserToProduct(user.Id, _productId);
                user.Subscriptions.Add(subscription);
            }

            await _organisationsApiClient.Update(new UpdateOrganisationRequest
            {
                EndPointAssessorName = organisation.EndPointAssessorName,
                EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                EndPointAssessorUkprn = organisation.EndPointAssessorUkprn,
                ApiEnabled = true,
                ApiUser = userId.ToString()
            });

            return user;
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
            var user = await GetUserDetails(userId);

            if (user != null)
            {
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/users/{userId}?api-version=2017-03-01&deleteSubscriptions=true"))
                {
                    httpRequest.Headers.Add("If-Match", "*");
                    await Delete(httpRequest);
                }

                if (!string.IsNullOrWhiteSpace(user.Note))
                {
                    string ukprn = user.Note.Trim().ToLower()
                                    .Replace("ukprn=", string.Empty);

                    var organisation = await _organisationsApiClient.Get(ukprn);
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

        public async Task<IEnumerable<AzureProduct>> ListProducts()
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/products?api-version=2017-03-01'"))
            {
                var response = await RequestAndDeserialiseAsync<AzureProductResponse>(httpRequest, "Could not get Products");
                return response.Products;
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
