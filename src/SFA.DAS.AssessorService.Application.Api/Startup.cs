using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.AssessmentOrgs.Api.Client.Core;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ViewModel.Models;
using StructureMap;
using Swashbuckle.AspNetCore.Swagger;
using SFA.DAS.AssessorService.Data.TestData;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        private const string ServiceName = "SFA.DAS.AssessorService";
        private const string Version = "1.0";

        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
            Configuration = ConfigurationService.GetConfig(_config["Environment"], _config["ConnectionStrings:Storage"], Version, ServiceName).Result;
        }

        public IWebConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "sfa.das.assessorservice",
                        ValidAudience = "sfa.das.assessorservice.api",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration.Api.TokenEncodingKey))
                    };
                });

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
                     opts => { opts.ResourcesPath = "Resources"; })
                    .AddDataAnnotationsLocalization();

            services
                .AddMvc()
                .AddControllersAsServices()
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "SFA.DAS.AssessorService.Application.Api", Version = "v1" });
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "SFA.DAS.AssessorService.Application.Api.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-GB")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("en-GB");
                    // Formatting numbers, dates, etc.
                    opts.SupportedCultures = supportedCultures;
                    // UI strings that we have localized.
                    opts.SupportedUICultures = supportedCultures;
                });

            var serviceProvider = ConfigureIOC(services);

            TestDataService.AddTestData(serviceProvider.GetService<AssessorDbContext>());

            return serviceProvider;
        }

        private IServiceProvider ConfigureIOC(IServiceCollection services)
        {
            var container = new Container();

            container.Configure(config =>
            {
                config.Scan(_ =>
                {
                    //_.AssemblyContainingType(typeof(Startup));
                    _.AssembliesFromApplicationBaseDirectory(c => c.FullName.StartsWith("SFA"));
                    _.WithDefaultConventions();

                    _.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
                    _.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
                    _.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });

                config.For<IWebConfiguration>().Use(Configuration);

                config.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
                config.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
                config.For<IMediator>().Use<Mediator>();
                config.For<IAssessmentOrgsApiClient>().Use(() => new AssessmentOrgsApiClient(null));

                var option = new DbContextOptionsBuilder<AssessorDbContext>();
                option.UseSqlServer(Configuration.SqlConnectionString);

                config.For<AssessorDbContext>().Use(c => new AssessorDbContext(option.Options, _env.IsDevelopment()));

                config.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SFA.DAS.AssessorService.Application.Api v1");
            });

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Organisation, OrganisationQueryViewModel>();
                cfg.CreateMap<OrganisationCreateViewModel, OrganisationCreateDomainModel>();
                cfg.CreateMap<OrganisationCreateDomainModel, Organisation>();
                cfg.CreateMap<Organisation, OrganisationQueryViewModel>();
            });

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}