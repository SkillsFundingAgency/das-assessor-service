using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Configuration;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers;
using SFA.DAS.AssessorService.Settings;
using StructureMap;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<Startup> _logger;
        private const string SERVICE_NAME = "SFA.DAS.AssessorService.ExternalApi";
        private const string VERSION = "1.0";
        private readonly bool _useSandbox;

        public Startup(IConfiguration configuration, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;
            _logger.LogInformation("In startup constructor.  Before Config");
            Configuration = configuration;

            if(!bool.TryParse(configuration["UseSandboxServices"], out _useSandbox))
            {
                _useSandbox = "yes".Equals(configuration["UseSandboxServices"], StringComparison.InvariantCultureIgnoreCase);
            }

            _logger.LogInformation($"UseSandbox is: {_useSandbox.ToString()}");
            _logger.LogInformation("In startup constructor.  After GetConfig");
        }

        public IConfiguration Configuration { get; }
        public IExternalApiConfiguration ApplicationConfiguration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                ApplicationConfiguration = ConfigurationService.GetConfigExternalApi(Configuration["EnvironmentName"], Configuration["ConfigurationStorageConnectionString"], VERSION, SERVICE_NAME).Result;

                services.AddAuthentication(auth =>
                {
                    auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(auth =>
                {
                    var validAudiences = new List<string>();
                    var tenant = string.Empty;
                    if (_useSandbox)
                    {
                        validAudiences.AddRange(ApplicationConfiguration.SandboxExternalApiAuthentication.Audiences.Split(","));
                        tenant = ApplicationConfiguration.SandboxExternalApiAuthentication.Tenant;
                    }
                    else
                    {
                        validAudiences.AddRange(ApplicationConfiguration.ExternalApiAuthentication.Audiences.Split(","));
                        tenant = ApplicationConfiguration.ExternalApiAuthentication.Tenant;
                    }
                    auth.Authority = $"https://login.microsoftonline.com/{tenant}";
                    auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                        ValidAudiences = validAudiences
                    };
                    auth.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = context => { return Task.FromResult(0); }
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

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = $"Assessor Service API {Configuration["InstanceName"]}", Version = "v1" });
                    c.EnableAnnotations();
                    c.ExampleFilters();
                    c.SchemaFilter<SwaggerRequiredSchemaFilter>();
                    c.CustomSchemaIds(x => x.FullName.Replace("SFA.DAS.AssessorService.Application.Api.External.Models.", ""));

                    if (_env.IsDevelopment())
                    {
                        var basePath = AppContext.BaseDirectory;
                        var xmlPath = Path.Combine(basePath, "SFA.DAS.AssessorService.Application.Api.External.xml");
                        c.IncludeXmlComments(xmlPath);
                    }
                });
                services.AddSwaggerExamplesFromAssemblyOf<Startup>();

                services.AddScoped<IHeaderInfo, HeaderInfo>();
                services.AddHttpContextAccessor();

                services.AddMvc(options =>
                {
                    options.Filters.Add(new AuthorizeFilter("default"));
                }).ConfigureApiBehaviorOptions(options =>
                    {
                        options.InvalidModelStateResponseFactory = context =>
                        {
                            try
                            {
                                var requestUrl = context.HttpContext.Request.Path;
                                var requestMethod = context.HttpContext.Request.Method;
                                var modelErrors = context.ModelState.SelectMany(model => model.Value.Errors.Select(err => err.ErrorMessage));
                                _logger.LogError($"Invalid request detected. {requestMethod.ToUpper()}: {requestUrl} - Errors: {string.Join(",", modelErrors)}");
                            }
                            catch
                            {
                                // safe to ignore!
                            }

                            var error = new ApiResponse((int)HttpStatusCode.Forbidden, "Your request contains invalid input. Please ensure it matches the swagger definition and try again.");
                            return new BadRequestObjectResult(error) { StatusCode = error.StatusCode };
                        };
                    }
                    )
                    .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Include;                        
                    });

                services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

                services.Configure<RequestLocalizationOptions>(
                    options =>
                    {
                        options.DefaultRequestCulture = new RequestCulture("en-GB");
                        options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
                        options.RequestCultureProviders.Clear();
                    });

                services.AddAutoMapper(typeof(Startup));

                return ConfigureIoC(services);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during Startup Configure Services");
                throw;
            }
        }

        private IServiceProvider ConfigureIoC(IServiceCollection services)
        {
            var container = new Container();

            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    _.AssembliesFromApplicationBaseDirectory(c => c.FullName.StartsWith("SFA"));
                    _.WithDefaultConventions();
                });

                if (_useSandbox)
                {
                    config.For<AssessorApiClientConfiguration>().Use(ApplicationConfiguration.SandboxAssessorApiAuthentication);
                    config.For<IApiClient>().Use<SandboxApiClient>();
                }
                else
                {
                    config.For<AssessorApiClientConfiguration>().Use(ApplicationConfiguration.AssessorApiAuthentication);
                    config.For<IApiClient>().Use<ApiClient>();
                }

                config.For<IExternalApiConfiguration>().Use(ApplicationConfiguration);

                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseHsts();
                }

                app.UseSwagger()
                    .UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Assessor Service API v1");
                    })
                    .UseAuthentication();

                if (_useSandbox)
                {
                    app.UseMiddleware<SandboxHeadersMiddleware>();
                }

                app.UseMiddleware<GetHeadersMiddleware>();

                app.UseHttpsRedirection();
                app.UseSecurityHeaders();

                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during Startup Configure");
                throw;
            }
        }
    }
}
