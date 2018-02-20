using System;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessmentOrgs.Api.Client.Core;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.RegisterUpdate;
using StructureMap;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class EpaoImporterFunction
    {
        [FunctionName("EpaoImporterFunction")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            var container = new Container(cfg =>
            {
                cfg.Scan(scan =>
                {
                    scan.Assembly("SFA.DAS.AssessorService.Application");
                    scan.Assembly("SFA.DAS.AssessorService.Application.Api.Client");
                    scan.Assembly("SFA.DAS.AssessmentOrgs.Api.Client.Core");
                    scan.WithDefaultConventions();

                    scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
                    scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
                    scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });

                cfg.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
                cfg.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
                cfg.For<IMediator>().Use<Mediator>();
                cfg.For<IAssessmentOrgsApiClient>().Use(() => new AssessmentOrgsApiClient(null));
                cfg.For<IOrganisationsApiClient>().Use<OrganisationsApiClient>();
                cfg.For<IOrganisationsApiClient>().Use<OrganisationsApiClient>().Ctor<string>().Is(Environment.GetEnvironmentVariable("Api:ApiBaseAddress", EnvironmentVariableTarget.Process));
            });

            var handler = container.GetInstance<IRequestHandler<RegisterUpdateRequest>>();

            var mediator = container.GetInstance<IMediator>();
            mediator.Send(new RegisterUpdateRequest());
        }
    }
}