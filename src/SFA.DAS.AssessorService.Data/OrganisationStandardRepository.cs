﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

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
                @"INSERT INTO OrganisationStandardVersion (StandardUId, Version ,OrganisationStandardId, EffectiveFrom, EffectiveTo, DateVersionApproved, Comments, Status)
                  VALUES (@StandardUId, @Version ,@OrganisationStandardId, @EffectiveFrom, @EffectiveTo, @DateVersionApproved, @Comments, @Status)",
                param: new { 
                    version.StandardUId, 
                    version.Version, 
                    version.OrganisationStandardId, 
                    version.EffectiveFrom, 
                    version.EffectiveTo, 
                    version.DateVersionApproved, 
                    version.Comments, 
                    version.Status 
                },
                transaction: _unitOfWork.Transaction);

            return version;
        }

        public async Task<OrganisationStandardVersion> GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(int organisationStandardId, string version)
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

        public async Task<OrganisationStandardVersion> UpdateOrganisationStandardVersion(OrganisationStandardVersion organisationStandardVersion)
        {
            var sql = @"UPDATE [OrganisationStandardVersion] 
                        SET
                            [EffectiveFrom] = @effectiveFrom,
                            [EffectiveTo] = @effectiveTo,
                            [DateVersionApproved] = COALESCE(DateVersionApproved, @dateVersionApproved),
                            [Comments] = @comments,
                            [Status] = @status
                        WHERE
                            [OrganisationStandardId] = @organisationStandardId AND [Version] = @version";

            await _unitOfWork.Connection.ExecuteAsync(
                sql,
                param: new {
                        version = organisationStandardVersion.Version,
                        organisationStandardId = organisationStandardVersion.OrganisationStandardId,
                        effectiveFrom = organisationStandardVersion.EffectiveFrom,
                        effectiveTo = organisationStandardVersion.EffectiveTo,
                        dateVersionApproved = organisationStandardVersion.DateVersionApproved,
                        comments = organisationStandardVersion.Comments,
                        status = organisationStandardVersion.Status
                },
                transaction: _unitOfWork.Transaction);

            return organisationStandardVersion;
        }

        public async Task WithdrawOrganisation(string endPointAssessorOrganisationId, DateTime withdrawalDate)
        {
            var sql = @"UPDATE osv
	                    SET
		                    [EffectiveTo] = @withdrawalDate
	                    FROM [OrganisationStandardVersion] osv 
		                    INNER JOIN [OrganisationStandard] os ON os.[Id] = osv.[OrganisationStandardId]
	                    WHERE os.[EndPointAssessorOrganisationId] = @endPointAssessorOrganisationId
		                    AND (osv.[EffectiveTo] IS NULL OR osv.[EffectiveTo] > @withdrawalDate);

                        UPDATE [OrganisationStandard]
	                    SET
		                    [EffectiveTo] = @withdrawalDate
	                    WHERE [EndPointAssessorOrganisationId] = @endPointAssessorOrganisationId
		                    AND ([EffectiveTo] IS NULL OR [EffectiveTo] > @withdrawalDate)";

            await _unitOfWork.Connection.ExecuteAsync(
                sql,
                param: new
                {
                    endPointAssessorOrganisationId,
                    withdrawalDate
                },
                transaction: _unitOfWork.Transaction);
        }

        public async Task WithdrawStandard(string endPointAssessorOrganisationId, int standardCode, DateTime withdrawalDate)
        {
            var sql = @"UPDATE osv
	                    SET
		                    [EffectiveTo] = @withdrawalDate
	                    FROM [OrganisationStandardVersion] osv 
		                    INNER JOIN [OrganisationStandard] os ON os.[Id] = osv.[OrganisationStandardId]
	                    WHERE os.[EndPointAssessorOrganisationId] = @endPointAssessorOrganisationId
                            AND os.[StandardCode] = @standardCode
		                    AND (osv.[EffectiveTo] IS NULL OR osv.[EffectiveTo] > @withdrawalDate);

                        UPDATE [OrganisationStandard]
	                    SET
		                    [EffectiveTo] = @withdrawalDate
	                    WHERE [EndPointAssessorOrganisationId] = @endPointAssessorOrganisationId
                            AND [StandardCode] = @standardCode
		                    AND ([EffectiveTo] IS NULL OR [EffectiveTo] > @withdrawalDate)";

            await _unitOfWork.Connection.ExecuteAsync(
                sql,
                param: new
                {
                    endPointAssessorOrganisationId,
                    standardCode,
                    withdrawalDate
                },
                transaction: _unitOfWork.Transaction);
        }
    }
}
