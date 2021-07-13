using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class OrganisationStandardRepository : Repository, IOrganisationStandardRepository
    {
        public OrganisationStandardRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
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

        public async Task<OrganisationStandardVersion> GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(int organisationStandardId, decimal version)
        {
            var sql = @"SELECT StandardUId, Version ,OrganisationStandardId, EffectiveFrom, EffectiveTo, DateVersionApproved, 
                            Comments, Status
                        FROM OrganisationStandardVersion 
                        WHERE OrganisationStandardId = @organisationStandardId AND Version = @version";

            var results = await _unitOfWork.Connection.QueryAsync<OrganisationStandardVersion>(
                sql,
                param: new { organisationStandardId, version},
                transaction: _unitOfWork.Transaction);

            return results.FirstOrDefault();
        }

        public async Task<OrganisationStandardVersion> UpdateOrganisationStandardVersion(OrganisationStandardVersion orgStandardVersion)
        {
            var sql = @"UPDATE [OrganisationStandardVersion] 
                        SET
                            [EffectiveFrom] = @effectiveFrom,
                            [EffectiveTo] = @effectiveTo,
                            [DateVersionApproved] = @dateVersionApproved,
                            [Status] = @status
                        WHERE
                            [OrganisationStandardId] = @orgStandardId AND [Version] = @version";

            await _unitOfWork.Connection.ExecuteAsync(
                sql,
                param: new { 
                        effectiveFrom = orgStandardVersion.EffectiveFrom,
                        effectiveTo = orgStandardVersion.EffectiveTo,
                        dateVersionApproved = orgStandardVersion.DateVersionApproved,
                        Status = orgStandardVersion.Status,
                        orgStandardId = orgStandardVersion.OrganisationStandardId,
                        version = orgStandardVersion.Version
                    },
                transaction: _unitOfWork.Transaction);

            return orgStandardVersion;
        }
    }
}
