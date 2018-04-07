using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data
{
    public class CertificatesRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;

        public CertificatesRepository(IConfiguration configuration,
            IDbConnection dbConnection)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _dbConnection = dbConnection;
        }

        public IEnumerable<Certificate> GetData()
        {
            var certificates = _dbConnection.Query<Certificate>
                ($@"SELECT [Id],
                    [CertificateData],
                    [CreatedAt],
                    [CreatedBy],
                    [DeletedAt],
                    [DeletedBy],
                    [CertificateReference],
                    [OrganisationId],
                    [Status],
                    [UpdatedAt],
                    [UpdatedBy],
                    [Uln],
                    [StandardCode],
                    [ProviderUkPrn],
                    [CertificateReferenceId]
                    FROM[Certificates] 
                    WHERE Status <> '{CertificateStatus.Printed}'").ToList();
            return certificates;
        }
    }
}
