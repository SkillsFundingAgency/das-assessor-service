using System;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class TokenService : ITokenService
    {
        private readonly ICache _cache;
        private readonly IWebConfiguration _configuration;
        private IDateTimeProvider _dateTimeProvider;

        public TokenService(ICache cache, IWebConfiguration configuration, IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _cache = cache;
            _configuration = configuration;
        }

        public string GetJwt(string userKey)
        {
            //var ukprn = _contextAccessor.HttpContext.User
            //    .FindFirst("http://schemas.portal.com/ukprn").Value;

            var result = _cache.GetString(userKey);

            //if (result == null)
            //{
            //    result = GetNewToken(userKey);
            //    _cache.SetString(userKey, result);
            //}
            //else
            //{
            //    try
            //    {
            //        new JwtBuilder()
            //            .WithSecret(_configuration.Api.TokenEncodingKey)
            //            .WithDateTimeProvider(_dateTimeProvider)
            //            .MustVerifySignature()                        
            //            .Decode(result);
            //    }
            //    catch (TokenExpiredException expired)
            //    {
            //        result = GetNewToken(userKey);
            //        _cache.SetString(userKey, result);
            //        return result;
            //    }
            //}
            
            return result;
        }

        //private string GetNewToken(string ukprn)
        //{
        //    var token = new JwtBuilder().WithAlgorithm(new HMACSHA256Algorithm())
        //        .WithSecret(_configuration.Api.TokenEncodingKey)
        //        .Issuer("sfa.das.assessorservice")
        //        .Audience("sfa.das.assessorservice.api")
        //        .ExpirationTime(_dateTimeProvider.GetNow().DateTime.AddMinutes(30))
        //        .AddClaim("ukprn", ukprn)
        //        .Build();

        //    return token;
        //}
    }
}