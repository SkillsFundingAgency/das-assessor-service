namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    using StructureMap;

    public class Bootstrapper
    {
        public static void Initialise()
        {
            Container = new Container(_ =>
            {
                _.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });
            });
        }

        public static Container Container { get;  private set; }
    }
}
