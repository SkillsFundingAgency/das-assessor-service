using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.ReprintTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Reprint console");
            Console.Write("Enter App Key: ");
            var appKey = Console.ReadLine();
            Console.Write("Enter Certificate Reference: ");
            var certRef = Console.ReadLine();
            Console.Write("Enter Achievement Date (dd/mm/yyyy): ");
            var achievementDate = Console.ReadLine();
            Console.Write("Enter Surname: ");
            var surname = Console.ReadLine();
            Console.Write("Enter Username (can be anything): ");
            var username = Console.ReadLine();


            var baseUri = "https://at-assessors-api.apprenticeships.sfa.bis.gov.uk/";
            var httpClient = new HttpClient { BaseAddress = new Uri($"{baseUri}") };

            var reprintRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/certificates/requestreprint");

            reprintRequest.Content = new StringContent(JsonConvert.SerializeObject(new CertificateReprintRequest
                {
                    CertificateReference = certRef,
                    AchievementDate = DateTime.Parse(achievementDate),
                    Username = username,
                    LastName = surname
                }),
                System.Text.Encoding.UTF8, "application/json");

            reprintRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(appKey));

            var response = httpClient.SendAsync(reprintRequest).Result;

            Console.WriteLine("Response received");
            Console.WriteLine($"Status code: {response.StatusCode}");
            Console.WriteLine("Content:");
            Console.Write(response.Content.ReadAsStringAsync().Result);
            Console.WriteLine();
            Console.WriteLine("Press Any Key to finish");
            Console.ReadKey();
        }

        public static string GetToken(string appKey)
        {
            var tenantId = "1a92889b-8ea1-4a16-8132-347814051567";
            var clientId = "b29803ca-cf9f-4b9a-9f63-a0e2700c55d7";
            var resourceId = "https://citizenazuresfabisgov.onmicrosoft.com/assessorservice-api";

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var tokenResult = context.AcquireTokenAsync(resourceId, clientCredential).Result;
            var accessToken = tokenResult.AccessToken;

            return accessToken;
        }
    }  
}