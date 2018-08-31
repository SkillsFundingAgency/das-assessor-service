using System.Configuration;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts;
using StructureMap;
using Configuration = SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts.Configuration;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    public class Bootstrapper
    {
        public static void Initialise()
        {           
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
