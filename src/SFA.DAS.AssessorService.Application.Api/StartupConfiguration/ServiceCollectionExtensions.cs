using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Apply;
using SFA.DAS.AssessorService.Data.Staff;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Azure;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.CharityCommission;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.CompaniesHouse;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.QnA;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.ReferenceData;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using System;
using System.Net.Http.Headers;
using SFA.DAS.AssessorService.Application.Api.Validators;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.IO;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using SFA.DAS.AssessorService.Domain.Helpers;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Application.Api.Services.Validation;
using static CharityCommissionService.SearchCharitiesV1SoapClient;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBaseConfiguration(this IServiceCollection services, IApiConfiguration apiConfig)
        {
            services.AddSingleton<IApiConfiguration>(apiConfig);
            services.AddSingleton<RoatpApiClientConfiguration>(apiConfig.RoatpApiAuthentication);
            services.AddSingleton<QnaApiClientConfiguration>(apiConfig.QnaApiAuthentication);
            services.AddSingleton<ReferenceDataApiClientConfiguration>(apiConfig.ReferenceDataApiAuthentication);
            services.AddSingleton<ICharityCommissionApiClientConfiguration>(apiConfig.CharityCommissionApiAuthentication);
            services.AddSingleton<ICompaniesHouseApiClientConfiguration>(apiConfig.CompaniesHouseApiAuthentication);
            services.AddSingleton<IOuterApiClientConfiguration>(apiConfig.OuterApi);

            return services;
        }

        public static IServiceCollection AddCustomControllers(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddControllers(options =>
            {
                if (env.IsDevelopment())
                {
                    options.Filters.Add(new AllowAnonymousFilter());
                }
            })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts =>
                {
                    opts.ResourcesPath = "Resources";
                })
                .AddDataAnnotationsLocalization()
                .AddNewtonsoftJson();

            return services;
        }

        public static IServiceCollection AddIisServerOptions(this IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });
            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, bool useSandbox, IApiConfiguration config)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                   .AddJwtBearer(o =>
                   {
                       var validAudiences = new List<string>();
                       var authority = string.Empty;

                       if (useSandbox)
                       {
                           validAudiences.AddRange(config.SandboxApiAuthentication.Audiences.Split(","));
                           authority = o.Authority = config.SandboxApiAuthentication.Tenant;
                       }
                       else
                       {
                           validAudiences.AddRange(config.ApiAuthentication.Audiences.Split(","));
                           authority = o.Authority = config.ApiAuthentication.Tenant;
                       }

                       o.Authority = $"https://login.microsoftonline.com/{authority}";
                       o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                       {
                           RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                           ValidAudiences = validAudiences
                       };
                       o.Events = new JwtBearerEvents()
                       {
                           OnTokenValidated = context => { return Task.FromResult(0); }
                       };
                   });

            services.AddTransient<JwtBearerTokenGenerator>();
            return services;
        }

        public static IServiceCollection AddCustomLocalization(this IServiceCollection services)
        {
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-GB");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                options.RequestCultureProviders.Clear();
            });

            return services;
        }

        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IHostEnvironment env)
        {
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "SFA.DAS.AssessorService.Application.Api", Version = "v1" });
                config.CustomSchemaIds(i => i.FullName);

                if (env.IsDevelopment())
                {
                    var basePath = AppContext.BaseDirectory;
                    var xmlPath = Path.Combine(basePath, "SFA.DAS.AssessorService.Application.Api.xml");
                    config.IncludeXmlComments(xmlPath);
                }

                if (!env.IsDevelopment())
                {
                    config.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });

                    config.OperationFilter<SecurityRequirementsOperationFilter>();
                }
            });
            return services;
        }

        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddHostedService<TaskQueueHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<Startup>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.FullName.StartsWith("SFA"))
                    .ToArray()
            ));
            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<CacheService>();
            services.AddTransient<IStandardService, StandardService>();
            services.AddTransient<IStandardImportService, StandardImportService>();
            services.AddTransient<IAzureTokenService, AzureTokenService>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<ICertificateNameCapitalisationService, CertificateNameCapitalisationService>();
            services.AddTransient<IOuterApiService, OuterApiService>();
            services.AddTransient<IEpaOrganisationIdGenerator, EpaOrganisationIdGenerator>();
            return services;
        }

        public static IServiceCollection AddHttpAndApiClients(this IServiceCollection services,
            IApiConfiguration apiConfiguration, Notifications.Api.Client.Configuration.INotificationsApiClientConfiguration notificationConfig)
        {
            services.AddHttpClient<ICompaniesHouseApiClient, CompaniesHouseApiClient>(config =>
            {
                config.BaseAddress = new Uri(apiConfiguration.CompaniesHouseApiAuthentication.ApiBaseAddress);
                config.DefaultRequestHeaders.Add("Accept", "Application/json");
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            services.AddHttpClient<IOuterApiClient, OuterApiClient>(config =>
            {
                var baseUrl = apiConfiguration.OuterApi.BaseUrl;
                config.BaseAddress = new Uri(baseUrl);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            services.AddHttpClient<INotificationsApi, NotificationsApi>((serviceProvider, client) =>
            {
                if (string.IsNullOrWhiteSpace(notificationConfig.ClientId))
                {
                    var tokenGenerator = serviceProvider.GetRequiredService<JwtBearerTokenGenerator>();
                    var token = tokenGenerator.Generate().Result;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    var aadTokenGenerator = new AzureActiveDirectoryBearerTokenGenerator(notificationConfig);
                    var token = aadTokenGenerator.Generate().Result;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                client.BaseAddress = new Uri(notificationConfig.ApiBaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddSingleton<CharityCommissionService.ISearchCharitiesV1SoapClient>(sp =>
                new CharityCommissionService.SearchCharitiesV1SoapClient(EndpointConfiguration.SearchCharitiesV1Soap)
            );

            services.AddTransient<IAssessorApiClientFactory, AssessorApiClientFactory>();
            services.AddTransient<IReferenceDataApiClientFactory, ReferenceDataApiClientFactory>();
            services.AddTransient<IRoatpApiClientFactory, RoatpApiClientFactory>();
            services.AddTransient<IQnaApiClientFactory, QnaApiClientFactory>();

            services.AddSingleton<IReferenceDataApiClient, ReferenceDataApiClient>();
            services.AddSingleton<IRoatpApiClient, RoatpApiClient>();
            services.AddSingleton<ICharityCommissionApiClient, CharityCommissionApiClient>();
            services.AddSingleton<IQnaApiClient, QnaApiClient>();
            services.AddSingleton<ISpecialCharacterCleanserService, SpecialCharacterCleanserService>();
            services.AddSingleton<IOrganisationsApiClient, OrganisationsApiClient>();
            services.AddSingleton<IContactsApiClient, ContactsApiClient>();
            services.AddSingleton<IFrameworkSearchApiClient, FrameworkSearchApiClient>();

            return services;
        }
        public static IServiceCollection AddHelpers(this IServiceCollection services)
        {
            services.AddTransient<IDateTimeHelper, DateTimeHelper>();
            return services;
        }

        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddTransient<IRegisterRepository, RegisterRepository>();
            services.AddTransient<IStaffLearnerRepository, StaffLearnerRepository>();
            services.AddTransient<IStandardRepository, StandardRepository>();
            services.AddTransient<IOrganisationQueryRepository, OrganisationQueryRepository>();
            services.AddTransient<IStaffCertificateRepository, StaffCertificateRepository>();
            services.AddTransient<IContactQueryRepository, ContactQueryRepository>();
            services.AddTransient<IBatchLogRepository, BatchLogRepository>();
            services.AddTransient<IOppFinderRepository, OppFinderRepository>();
            services.AddTransient<IContactRepository, ContactRepository>();
            services.AddTransient<ICertificateRepository, CertificateRepository>();
            services.AddTransient<IRegisterQueryRepository, RegisterQueryRepository>();
            services.AddTransient<IDashboardRepository, DashboardRepository>();
            services.AddTransient<IEMailTemplateQueryRepository, EMailTemplateQueryRepository>();
            services.AddTransient<IOrganisationRepository, OrganisationRepository>();
            services.AddTransient<ISettingRepository, SettingRepository>();
            services.AddTransient<IRegisterValidationRepository, RegisterValidationRepository>();
            services.AddTransient<ISandboxDbRepository, SandboxDbRepository>();
            services.AddTransient<IBatchLogQueryRepository, BatchLogQueryRepository>();
            services.AddTransient<IOrganisationStandardRepository, OrganisationStandardRepository>();
            services.AddTransient<IIlrRepository, IlrRepository>();
            services.AddTransient<IApprovalsExtractRepository, ApprovalsExtractRepository>();
            services.AddTransient<IScheduleRepository, ScheduleRepository>();
            services.AddTransient<IProvidersRepository, ProvidersRepository>();
            services.AddTransient<ILearnerRepository, LearnerRepository>();
            services.AddTransient<IStaffReportRepository, StaffReportRepository>();
            services.AddTransient<IApplyRepository, ApplyRepository>();
            services.AddTransient<IFrameworkLearnerRepository, FrameworkLearnerRepository>();
            return services;
        }

        public static IServiceCollection RegisterValidators(this IServiceCollection services)
        {
            services.AddScoped<UkPrnValidator>();
            services.AddScoped<SearchOrganisationForContactsValidator>();
            services.AddScoped<IEpaOrganisationValidator, EpaOrganisationValidator>();
            services.AddScoped<IEpaOrganisationSearchValidator, EpaOrganisationSearchValidator>();
            return services;

        }
    }
}
