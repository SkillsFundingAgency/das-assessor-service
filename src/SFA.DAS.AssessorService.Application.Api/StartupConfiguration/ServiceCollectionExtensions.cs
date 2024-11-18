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
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using System;
using System.Configuration;
using System.Net.Http.Headers;
using static CharityCommissionService.SearchCharitiesV1SoapClient;
using SFA.DAS.AssessorService.Application.Api.Validators;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<CacheService>();
            services.AddTransient<IStandardService, StandardService>();
            services.AddTransient<IAzureTokenService, AzureTokenService>();
            return services;
        }

        public static IServiceCollection AddApiClients(this IServiceCollection services, Notifications.Api.Client.Configuration.INotificationsApiClientConfiguration notificationConfig)
        {
            services.AddTransient<IAssessorApiClientFactory, AssessorApiClientFactory>();
            services.AddTransient<IReferenceDataApiClientFactory, ReferenceDataApiClientFactory>();
            services.AddTransient<IRoatpApiClientFactory, RoatpApiClientFactory>();
            services.AddTransient<IQnaApiClientFactory, QnaApiClientFactory>();


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

            // This is a SOAP service. The client interfaces are contained within the generated proxy code
            services.AddSingleton<CharityCommissionService.ISearchCharitiesV1SoapClient>(sp =>
                new CharityCommissionService.SearchCharitiesV1SoapClient(EndpointConfiguration.SearchCharitiesV1Soap)
            );

            services.AddSingleton<IReferenceDataApiClient, ReferenceDataApiClient>();
            services.AddSingleton<IRoatpApiClient, RoatpApiClient>();
            services.AddSingleton<ICompaniesHouseApiClient, CompaniesHouseApiClient>();
            services.AddSingleton<ICharityCommissionApiClient, CharityCommissionApiClient>();
            services.AddSingleton<IQnaApiClient, QnaApiClient>();
            services.AddSingleton<ISpecialCharacterCleanserService, SpecialCharacterCleanserService>();
            services.AddSingleton<IOrganisationsApiClient, OrganisationsApiClient>();
            services.AddSingleton<IContactsApiClient, ContactsApiClient>();
            services.AddSingleton<IOuterApiClient, OuterApiClient>();
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
            return services;
        }

        public static IServiceCollection RegisterValidators(this IServiceCollection services)
        {
            services.AddScoped<UkPrnValidator>();
            services.AddScoped<SearchOrganisationForContactsValidator>();
            services.AddScoped<IEpaOrganisationValidator, EpaOrganisationValidator>();
            services.AddScoped<IEpaOrganisationSearchValidator, EpaOrganisationSearchValidator>();
            services.AddScoped<SearchOrganisationForContactsValidator>();
            return services;

        }
        public static void InspectAssemblies(this IServiceCollection services, bool includeExternalDependencies = true)
        {
            // Get all registered service types
            var registeredServices = services.Select(s => s.ServiceType).ToHashSet();

            // Get the current assembly
            var currentAssembly = typeof(Startup).Assembly;

            // Keep track of inspected types to avoid infinite loops
            var inspectedTypes = new HashSet<Type>();

            foreach (var type in currentAssembly.GetTypes())
            {
                InspectTypeDependencies(type, registeredServices, inspectedTypes, services, new Stack<Type>(), includeExternalDependencies);
            }
        }

        private static void InspectTypeDependencies(
            Type type,
            HashSet<Type> registeredServices,
            HashSet<Type> inspectedTypes,
            IServiceCollection services,
            Stack<Type> dependencyStack,
            bool includeExternalDependencies)
        {
            // Initialize the stack if it's the root call
            dependencyStack ??= new Stack<Type>();

            // Skip already inspected types
            if (!type.IsClass || type.IsAbstract || inspectedTypes.Contains(type))
                return;

            // Mark type as inspected
            inspectedTypes.Add(type);

            // Push the current type onto the dependency stack
            dependencyStack.Push(type);

            foreach (var constructor in type.GetConstructors())
            {
                foreach (var parameter in constructor.GetParameters())
                {
                    var parameterType = parameter.ParameterType;

                    // Skip logger parameters
                    if (string.Equals(parameter.Name, "logger", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Skip Microsoft interfaces
                    if (parameterType.IsInterface && IsMicrosoftAssembly(parameterType))
                        continue;

                    // Skip external dependencies if not included
                    if (!includeExternalDependencies && parameterType.Assembly != typeof(Startup).Assembly)
                        continue;

                    // Check if the parameter is an interface and is not registered
                    if (parameterType.IsInterface && !registeredServices.Contains(parameterType))
                    {
                        // Log unregistered interface with stack trace
                        Trace.WriteLine($"Unregistered interface: {parameterType.FullName}");
                        Trace.WriteLine("Dependency chain:");

                        foreach (var dependency in dependencyStack)
                        {
                            Trace.WriteLine($"  -> {dependency.FullName}");
                        }

                        Trace.WriteLine($"  -> {parameterType.FullName} (parameter: {parameter.Name})");
                    }

                    // If the parameter is a class, recursively check its dependencies
                    if (parameterType.IsClass && !parameterType.IsAbstract)
                    {
                        InspectTypeDependencies(parameterType, registeredServices, inspectedTypes, services, dependencyStack, includeExternalDependencies);
                    }
                }
            }

            // Pop the current type off the stack after processing
            dependencyStack.Pop();
        }


        private static bool IsMicrosoftAssembly(Type type)
        {
            // Check if the type's assembly is from Microsoft
            return type.Assembly.FullName?.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) == true ||
                   type.Assembly.FullName?.StartsWith("System.", StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}
