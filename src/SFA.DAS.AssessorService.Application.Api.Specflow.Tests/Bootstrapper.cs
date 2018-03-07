using System.Configuration;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts;
using StructureMap;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    public class Bootstrapper
    {
        public static void Initialise()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[PersistenceNames.AccessorDBConnectionString].ConnectionString;

            Container = new Container(configure =>
            {
                configure.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });
            });
        }

        public static Container Container { get; private set; }
    }
}
