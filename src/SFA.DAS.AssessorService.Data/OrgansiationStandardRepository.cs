using Dapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class OrgansiationStandardRepository : Repository, IOrgansiationStandardRepository
    {
        private readonly ILogger<OrgansiationStandardRepository> _logger;

        public OrgansiationStandardRepository(IUnitOfWork unitOfWork, ILogger<OrgansiationStandardRepository> logger)
            : base(unitOfWork)
        {
            _logger = logger;
        }

        public async Task<OrganisationStandard> GetOrganisationStandardByOrganisationIdAndStandardReference(string organisationId, string standardReference)
        {
            var results =  await _unitOfWork.Connection.QueryAsync<OrganisationStandard>(
                @"SELECT Id, EndPointAssessorOrganisationId, StandardCode, EffectiveFrom, EffectiveTo, DateStandardApprovedOnRegister, 
                  Comments, Status, ContactId, StandardReference
                FROM OrganisationStandard
                WHERE EndPointAssessorOrganisationId = @OrganisationId AND StandardReference = @StandardReference",
                param: new { OrganisationId = organisationId, StandardReference = standardReference },
                transaction: _unitOfWork.Transaction);

            return results.SingleOrDefault();
        }

        public async Task<OrganisationStandardVersion> CreateOrganisationStandardVersion(OrganisationStandardVersion version)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                @"INSERT INTO OrganisationStandardVersion 
                    (StandardUId, Version ,OrganisationStandardId, EffectiveFrom, EffectiveTo, DateVersionApproved, Comments, Status)
                  VALUES (@StandardUId, @Version ,@OrganisationStandardId, @EffectiveFrom, @EffectiveTo, @DateVersionApproved, @Comments, @Status)",
                param: new { version.StandardUId, version.Version, version.OrganisationStandardId, version.EffectiveFrom, version.EffectiveTo, version.DateVersionApproved, version.Comments, version.Status },
                transaction: _unitOfWork.Transaction);

            return version;
        }
    }
}
