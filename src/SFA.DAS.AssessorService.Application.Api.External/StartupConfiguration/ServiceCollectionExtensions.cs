using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Azure;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;

namespace SFA.DAS.AssessorService.Application.Api.External.StartupConfiguration
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddCustomAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration, bool useSandbox, IExternalApiConfiguration appConfig)
        {
            services.AddAuthentication(auth =>
            {
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(auth =>
            {
                var validAudiences = new List<string>();
                var tenant = string.Empty;

                if (useSandbox)
                {
                    validAudiences.AddRange(appConfig.SandboxExternalApiAuthentication.Audiences.Split(","));
                    tenant = appConfig.SandboxExternalApiAuthentication.Tenant;
                }
                else
                {
                    validAudiences.AddRange(appConfig.ExternalApiAuthentication.Audiences.Split(","));
                    tenant = appConfig.ExternalApiAuthentication.Tenant;
                }

                auth.Authority = $"https://login.microsoftonline.com/{tenant}";
                auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    ValidAudiences = validAudiences
                };
                auth.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context => Task.FromResult(0)
                };
            });

            services.AddAuthorization(o =>
            {
                o.AddPolicy("default", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("APIM");
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomLocalization(this IServiceCollection services)
        {
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            return services;
        }

        public static IServiceCollection AddHttpContextServices(this IServiceCollection services)
        {
            services.AddScoped<IHeaderInfo, HeaderInfo>();
            services.AddHttpContextAccessor();

            return services;
        }

        public static IServiceCollection AddCustomControllers(this IServiceCollection services, ILogger logger)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter("default"));
            })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        try
                        {
                            var requestUrl = context.HttpContext.Request.Path;
                            var requestMethod = context.HttpContext.Request.Method;
                            var modelErrors = context.ModelState.SelectMany(model => model.Value.Errors.Select(err => err.ErrorMessage));
                            logger.LogError($"Invalid request detected. {requestMethod.ToUpper()}: {requestUrl} - Errors: {string.Join(",", modelErrors)}");
                        }
                        catch
                        {
                            // safe to ignore!
                        }

                        var error = new ApiResponse((int)HttpStatusCode.Forbidden, "Your request contains invalid input. Please ensure it matches the swagger definition and try again.");
                        return new BadRequestObjectResult(error) { StatusCode = error.StatusCode };
                    };
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                });

            return services;
        }

        public static IServiceCollection AddApiClients(this IServiceCollection services, bool useSandbox, IExternalApiConfiguration appConfig)
        {
            if (useSandbox)
            {
                services.AddSingleton(appConfig.SandboxAssessorApiAuthentication);
                services.AddScoped<IApiClient, SandboxApiClient>();
            }
            else
            {
                services.AddSingleton(appConfig.AssessorApiAuthentication);
                services.AddScoped<IApiClient, ApiClient>();
            }

            services.AddTransient<IAssessorApiClientFactory, AssessorApiClientFactory>();
            services.AddSingleton<IExternalApiConfiguration>(appConfig);

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, string instanceName, IWebHostEnvironment env)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = $"Assessor Service API {instanceName}",
                    Version = "v1"
                });

                if (env.IsDevelopment())
                {
                    var basePath = AppContext.BaseDirectory;
                    var xmlPath = Path.Combine(basePath, "SFA.DAS.AssessorService.Application.Api.External.xml");
                    c.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }
    }
}
