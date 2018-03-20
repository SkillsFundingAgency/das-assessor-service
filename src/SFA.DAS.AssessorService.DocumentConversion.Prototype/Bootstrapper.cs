using StructureMap;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
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
