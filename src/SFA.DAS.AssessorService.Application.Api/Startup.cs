using System;
using System.IO;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.AssessmentOrgs.Api.Client.Core;
using StructureMap;
using StructureMap.Diagnostics.TreeView;
using Swashbuckle.AspNetCore.Swagger;

namespace SFA.DAS.AssessorService.Application.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
                            Encoding.UTF8.GetBytes(Configuration["AzureAd:TokenEncodingKey"]))  
                    };
                });


            
            //services.AddMediatR(Assembly.Load("SFA.DAS.AssessorService.Application"));
            services.AddMvc().AddControllersAsServices();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {Title = "SFA.DAS.AssessorService.Application.Api", Version = "v1"});
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "SFA.DAS.AssessorService.Application.Api.xml");
                c.IncludeXmlComments(xmlPath);
            });

            return ConfigureIOC(services);

            //services.AddTransient<IOrganisationRepository, OrganisationRepository>();
            //services.AddTransient<IAssessmentOrgsApiClient, AssessmentOrgsApiClient>();
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

                config.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
                config.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
                config.For<IMediator>().Use<Mediator>();
                config.For<IAssessmentOrgsApiClient>().Use(() => new AssessmentOrgsApiClient(null));

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

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
