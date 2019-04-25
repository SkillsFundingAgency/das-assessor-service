﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Client.Azure
{
    public class AzureApiClient : AzureApiClientBase, IAzureApiClient
    {
        private readonly string _groupId;
        private readonly string _productId;
        private readonly Uri _requestBaseUri;
        protected readonly IOrganisationsApiClient _organisationsApiClient;
        protected readonly IContactsApiClient _contactsApiClient;

        public AzureApiClient(string baseUri, IAzureTokenService tokenService, ILogger<AzureApiClientBase> logger, IWebConfiguration webConfiguration, IOrganisationsApiClient organisationsApiClient, IContactsApiClient contactsApiClient) : base(baseUri, tokenService, logger)
        {
            _productId = webConfiguration.AzureApiAuthentication.ProductId;
            _groupId = webConfiguration.AzureApiAuthentication.GroupId;
            _requestBaseUri = new Uri(webConfiguration.AzureApiAuthentication.RequestBaseAddress);
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

        public async Task<IEnumerable<AzureUser>> GetUserDetailsByUkprn(string ukprn, bool includeSubscriptions = false, bool includeGroups = false)
        {
            List<AzureUser> users = new List<AzureUser>();

            if (!string.IsNullOrWhiteSpace(ukprn))
            {
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/users?api-version=2017-03-01&expandGroups={includeGroups}&$filter=contains(note,'ukprn') and contains(note,'{ukprn}')"))
                {
                    var response = await RequestAndDeserialiseAsync<AzureUserResponse>(httpRequest, "Could not get Users");

                    foreach (var user in response.Users)
                    {
                        if (user.AzureId != null && includeSubscriptions)
                        {
                            var subscriptions = await GetSubscriptionsForUser(user.Id);
                            user.Subscriptions.AddRange(subscriptions);
                        }

                        users.Add(user);
                    }
                }
            }

            return users;
        }

        public async Task<AzureUser> GetUserDetailsByEmail(string email, bool includeSubscriptions = false, bool includeGroups = false)
        {
            AzureUser user = null;

            if (!string.IsNullOrWhiteSpace(email))
            {
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
                    subscription.ApiEndPointUrl = new Uri(_requestBaseUri, subscription.ProductId).ToString();
                }

                return response.Subscriptions.Where(sub => !sub.IsCancelled).AsEnumerable();
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
            AzureUser user = null;

            var organisation = await _organisationsApiClient.Get(ukprn);
            var contact = await _contactsApiClient.GetByUsername(username);

            IEnumerable<AzureUser> ukprnUsers = await GetUserDetailsByUkprn(ukprn, true, true);

            var ukprnUsersWithSubscription = (from u in ukprnUsers
                                              from subs in ukprnUsers.SelectMany(au => au.Subscriptions.Where(s => s.IsActive && s.ProductId == _productId))
                                              where u.Id == subs.UserId
                                              select u).ToList();

            AzureUser emailUser = await GetUserDetailsByEmail(contact?.Email, true, true);
            AzureUser ukprnUser = ukprnUsersWithSubscription.Where(u => u.Email.Equals(contact?.Email)).FirstOrDefault();
            
            if (ukprnUsersWithSubscription.Any() && ukprnUser is null)
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

                if (!user.Subscriptions.Any(s => string.Equals(s.ProductId, _productId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    await SubscribeUserToProduct(user.Id, _productId);
                }

                await _organisationsApiClient.Update(new UpdateOrganisationRequest
                {
                    EndPointAssessorName = organisation.EndPointAssessorName,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    EndPointAssessorUkprn = organisation.EndPointAssessorUkprn,
                    ApiEnabled = true,
                    ApiUser = user.Id
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
            var user = await GetUserDetails(userId, true);

            if (user != null)
            {
                foreach (var subscription in user.Subscriptions.Where(s => string.Equals(s.ProductId, _productId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    await EnableSubscription(subscription.Id);
                }

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
            var user = await GetUserDetails(userId, true);

            if (user != null)
            {
                foreach (var subscription in user.Subscriptions.Where(s => string.Equals(s.ProductId, _productId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    await DisableSubscription(subscription.Id);
                }

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
                var hasOtherSubscriptions = user.Subscriptions.Any(s => !string.Equals(s.ProductId, _productId, StringComparison.InvariantCultureIgnoreCase));

                if (!hasOtherSubscriptions)
                {
                    // No other subscriptions so do an aggressive delete
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
                }

                if (!string.IsNullOrWhiteSpace(user.Ukprn))
                {
                    var organisation = await _organisationsApiClient.Get(user.Ukprn);

                    if (organisation != null)
                    {
                        // If we have a result here then take the first result as the new api user
                        var apiUsers = await GetUserDetailsByUkprn(user.Ukprn);
                        var newApiUser = apiUsers.SelectMany(u => u.Subscriptions.Where(s => s.IsActive && s.ProductId == _productId)).FirstOrDefault();

                        await _organisationsApiClient.Update(new UpdateOrganisationRequest
                        {
                            EndPointAssessorName = organisation.EndPointAssessorName,
                            EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                            EndPointAssessorUkprn = organisation.EndPointAssessorUkprn,
                            ApiEnabled = newApiUser != null,
                            ApiUser = newApiUser?.UserId
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
