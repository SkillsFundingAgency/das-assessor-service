using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using StructureMap;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class Bootstrapper
    {
        public static void Initialise()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();       

            Container = new Container(configure =>
            {
                configure.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });

                configure.For<IConfiguration>().Use(configuration);
                configure.For<SftpClient>().Use<SftpClient>("Build ISession from ISessionFactory",
                    c => new SftpClient(configuration["Sftp:RemoteHost"], Convert.ToInt32(configuration["Sftp:Port"]), configuration["Sftp:Username"], configuration["Sftp:Password"]));
            });
        }

        public static Container Container { get; private set; }
    }
}
