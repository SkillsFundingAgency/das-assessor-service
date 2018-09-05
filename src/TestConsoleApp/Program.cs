using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Domain.Entities;
using TestConsoleApp.Models;
using CertificateAddress = SFA.DAS.AssessorService.Domain.Entities.CertificateAddress;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var option = new DbContextOptionsBuilder<AssessorDbContext>();
            var connectionString =
                "Data Source=.\\SQLEXPRESS;Initial Catalog=SFA.DAS.AssessorService;Integrated Security=True;";
            option.UseSqlServer(connectionString);

            using (var assessorDbContext = new AssessorDbContext(option.Options))
            {
                var organisations = assessorDbContext.Organisations.FromSql("usp_GetAllOrganisations").ToList();

               
            }

            var connection = new SqlConnection(connectionString);

            var orgs = connection.Query<Organisation>("usp_GetAllOrganisations",
                commandType: CommandType.StoredProcedure).ToList();

            var organisationId = new Guid("DA488EB4-E91D-492B-0442-08D5D6A9D12A");

            var addresses = connection.Query<CertificateAddress>("GetRecentCertificateAddresses",
                new { OrganisationId = organisationId },
                commandType: CommandType.StoredProcedure).ToList();
        }
    }
}
