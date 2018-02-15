//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.OpenIdConnect;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;

//namespace SFA.DAS.AssessorService.Web.Extensions
//{
//    public static class AuthenticationBuilderExtensions
//    {        
//        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder)
//            => builder.AddAzureAd(_ => { });

//        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder, Action<AuthOptions> configureOptions)
//        {
//            builder.Services.Configure(configureOptions);
//            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAuthenticationOptions>();
//            builder.AddOpenIdConnect();
//            return builder;
//        }

//        private class ConfigureAuthenticationOptions: IConfigureNamedOptions<OpenIdConnectOptions>
//        {
//            private readonly AuthOptions _options;

//            public ConfigureAuthenticationOptions(IOptions<AuthOptions> options)
//            {
//                _options = options.Value;
//            }

//            public void Configure(string name, OpenIdConnectOptions options)
//            {
//                options.ClientId = _options.ClientId;
//                options.Authority = $"{_options.Instance}{_options.TenantId}";
//                options.UseTokenLifetime = true;
//                options.CallbackPath = _options.CallbackPath;
//                options.RequireHttpsMetadata = false;
//                options.ClientSecret = _options.ClientSecret;
//                options.ResponseType = "id_token code";
//                options.Resource = "https://graph.windows.net"; // AAD graph
                
//                options.Events.OnTokenValidated = OnTokenValidated;
//            }

//            private Task OnTokenValidated(TokenValidatedContext context)
//            {
//                var userObjectId = (context.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

//                // get ukprn *hopefully* from idams claims. Currently using the objectId as the ukprn.
//                var claims = new[]
//                {
//                    new Claim("ukprn", userObjectId, ClaimValueTypes.String)
//                };

//                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.TokenEncodingKey));
//                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//                var token = new JwtSecurityToken(
//                    issuer: "sfa.das.assessorservice",
//                    audience: "sfa.das.assessorservice.api",
//                    claims: claims,
//                    expires: DateTime.Now.AddSeconds(30),
//                    signingCredentials: creds);

//                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                
//                context.HttpContext.Session.SetString(userObjectId, jwt);

//                return Task.FromResult(0);
//            }

//            public void Configure(OpenIdConnectOptions options)
//            {
//                Configure(Options.DefaultName, options);
//            }
//        }
//    }
//}
