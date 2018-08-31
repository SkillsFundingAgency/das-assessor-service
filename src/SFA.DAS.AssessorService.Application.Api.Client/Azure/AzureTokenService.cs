using SFA.DAS.AssessorService.Settings;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;


namespace SFA.DAS.AssessorService.Application.Api.Client.Azure
{
    public class AzureTokenService : IAzureTokenService
    {
        private readonly IWebConfiguration _configuration;

        public AzureTokenService(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken()
        {
            DateTime expiry = DateTime.UtcNow.AddDays(1);
            string id = _configuration.AzureApiAuthentication.Id;
            string key = _configuration.AzureApiAuthentication.Key;

            using (var encoder = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                string dataToSign = id + "\n" + expiry.ToString("O", CultureInfo.InvariantCulture);
                string x = string.Format("{0}\n{1}", id, expiry.ToString("O", CultureInfo.InvariantCulture));
                byte[] hash = encoder.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                string signature = Convert.ToBase64String(hash);

                return string.Format("uid={0}&ex={1:o}&sn={2}", id, expiry, signature);
            }
        }
    }
}
