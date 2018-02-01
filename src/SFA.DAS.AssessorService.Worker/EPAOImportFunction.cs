using System;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using StructureMap;

namespace SFA.DAS.AssessorService.Worker
{
    public static class EPAOImportFunction
    {
        private static Container _container;

        [FunctionName("EPAOImportFunction")]
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info("Checking EPAO Register....");

            _container = new Container(cfg =>
                {
                    cfg.Scan(scan =>
                    {
                        scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                        scan.WithDefaultConventions();
                        scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>));
                        scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                        scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                    });
                    cfg.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
                    cfg.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
                    cfg.For<IMediator>().Use<Mediator>();
                }
            );

            log.Info("Container set up");
        }
    }
}
