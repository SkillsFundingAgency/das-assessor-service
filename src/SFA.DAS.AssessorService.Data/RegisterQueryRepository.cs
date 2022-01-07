using Dapper;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterQueryRepository : Repository, IRegisterQueryRepository
    {
        public RegisterQueryRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            SqlMapper.AddTypeHandler(typeof(ApplyData), new ApplyDataHandler());
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
            SqlMapper.AddTypeHandler(typeof(OrganisationStandardData), new OrganisationStandardDataHandler());
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypes()
        {
            var orgTypes = await _unitOfWork.Connection.QueryAsync<OrganisationType>(
                @"select * from [OrganisationType] where Status <> 'Deleted' order by id", 
                transaction: _unitOfWork.Transaction);
            
            return orgTypes;
        }

        public async Task<IEnumerable<DeliveryArea>> GetDeliveryAreas()
        {
            var deliveryAreas = await _unitOfWork.Connection.QueryAsync<DeliveryArea>(
                "select * from [DeliveryArea] order by ordering");

            return deliveryAreas;
        }

        public async Task<EpaOrganisation> GetEpaOrganisationById(Guid id)
        {
            var sql =
                "SELECT O.Id, O.CreatedAt, O.DeletedAt, O.EndPointAssessorName as Name, O.EndPointAssessorOrganisationId as OrganisationId, O.EndPointAssessorUkprn as ukprn, " +
                    "O.PrimaryContact, C.DisplayName as PrimaryContactName, O.Status, O.UpdatedAt, O.OrganisationTypeId, O.OrganisationData, O.ApiEnabled, O.ApiUser " +
                    " FROM [Organisations] O " +
                    "LEFT OUTER JOIN [Contacts] C ON C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "WHERE O.Id = @id";

            var org = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<EpaOrganisation>(sql, new { id });

            return org;
        }

        public async Task<EpaOrganisation> GetEpaOrganisationByOrganisationId(string organisationId)
        {
            var sql =
                "SELECT O.Id, O.CreatedAt, O.DeletedAt, O.EndPointAssessorName as Name, O.EndPointAssessorOrganisationId as OrganisationId, O.EndPointAssessorUkprn as ukprn, " +
                    "O.PrimaryContact, C.DisplayName as PrimaryContactName, O.Status, O.UpdatedAt, O.OrganisationTypeId, O.OrganisationData, O.ApiEnabled, O.ApiUser " +
                    " FROM [Organisations] O " +
                    "LEFT OUTER JOIN [Contacts] C ON C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "WHERE O.EndPointAssessorOrganisationId = @organisationId";

            var org = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<EpaOrganisation>(sql, new { organisationId });
            
            return org;
        }

        public async Task<string> EpaOrganisationIdCurrentMaximum()
        {
            var sql = @"select max(CAST(replace(EndPointAssessorOrganisationId,'EPA','') AS int)) OrgId from organisations where EndPointAssessorOrganisationId like 'EPA%' 
                        and isnumeric(replace(EndPointAssessorOrganisationId,'EPA','')) = 1";

            return await _unitOfWork.Connection.ExecuteScalarAsync<string>(sql);
        }

        public async Task<int> EpaContactUsernameHighestCounter()
        {
            var sql = "select max(convert(int,replace(username,'unknown-',''))) highestCounter from [Contacts] where username like 'unknown-%' and isnumeric(replace(username,'unknown-','')) = 1";

            var maxCounter = await _unitOfWork.Connection.ExecuteScalarAsync<int?>(sql);

            return maxCounter ?? 100;
        }

        public async Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisations()
        {
            var assessmentOrganisationSummaries =
                await _unitOfWork.Connection.QueryAsync<AssessmentOrganisationSummary>(
                    "select EndPointAssessorOrganisationId as Id, " +
                        "EndPointAssessorName as Name, " +
                        "EndPointAssessorUkprn as ukprn, " +
                        "OrganisationData, " +
                        "OrganisationTypeId, " +
                        "ot.Type as OrganisationType, " +
                        "o.Status, " +
                        "JSON_VALUE(OrganisationData,'$.Email') as Email " +
                        "from [Organisations] o " +
                        "left join OrganisationType ot " +
                        "on ot.id = o.OrganisationTypeId " +
                        "and ot.Status = 'Live'");

            return assessmentOrganisationSummaries;
        }

        public async Task<IEnumerable<AssessmentOrganisationContact>> GetAssessmentOrganisationContacts(string organisationId)
        {
            var sql =
                "SELECT C.Id, C.EndPointAssessorOrganisationId as OrganisationId, C.CreatedAt, C.DeletedAt, " +
                    "C.DisplayName, C.email, C.Status, C.UpdatedAt, C.Username, C.PhoneNumber, " +
                    "CASE WHEN PrimaryContact Is NULL THEN 0 ELSE 1 END AS IsPrimaryContact " +
                    "from contacts C  left outer join Organisations O on " +
                    "C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "where C.EndPointAssessorOrganisationId = @organisationId " +
                    "order by CASE WHEN PrimaryContact Is NULL THEN 0 ELSE 1 END DESC";

            return await _unitOfWork.Connection.QueryAsync<AssessmentOrganisationContact>(sql, new { organisationId });
        }

        public async Task<AssessmentOrganisationContact> GetAssessmentOrganisationContact(Guid contactId)
        {
            var sql =
                "SELECT C.Id, C.EndPointAssessorOrganisationId as OrganisationId, C.CreatedAt, C.DeletedAt, " +
                    "C.DisplayName, C.email, C.Status, C.UpdatedAt, C.Username, C.PhoneNumber, C.GivenNames as FirstName, C.FamilyName as LastName, " +
                    "CASE WHEN PrimaryContact Is NULL THEN 0 ELSE 1 END AS IsPrimaryContact " +
                    "from contacts C  left outer join Organisations O on " +
                    "C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "where convert(varchar(50),C.Id) = @contactId ";

            var contact = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<AssessmentOrganisationContact>(sql, new { contactId });

            return contact;
        }

        public async Task<AssessmentOrganisationContact> GetPrimaryOrFirstContact(string organisationId)
        {
            var sql =
                "SELECT top 1 C.Id, C.EndPointAssessorOrganisationId as OrganisationId, C.CreatedAt, C.DeletedAt, " +
                    "C.DisplayName, C.email, C.Status, C.UpdatedAt, C.Username, C.PhoneNumber, " +
                    "CASE WHEN PrimaryContact Is NULL THEN 0 ELSE 1 END AS IsPrimaryContact " +
                    "from contacts C  left outer join Organisations O on " +
                    "C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "where C.EndPointAssessorOrganisationId = @organisationId " +
                    "order by CASE WHEN PrimaryContact Is NULL THEN 0 ELSE 1 END DESC";

            var contact = await _unitOfWork.Connection.QuerySingleAsync<AssessmentOrganisationContact>(sql, new { organisationId });

            return contact;
        }

        public async Task<IEnumerable<EpaOrganisation>> GetAssessmentOrganisationsByStandardId(int standardId)
        {
            var sql =
                "SELECT O.Id, O.CreatedAt, O.DeletedAt, O.EndPointAssessorName as Name,  O.EndPointAssessorOrganisationId as OrganisationId, O.EndPointAssessorUkprn as ukprn, " +
                    "O.PrimaryContact, C.DisplayName as PrimaryContactName, O.Status, O.UpdatedAt, O.OrganisationTypeId, O.OrganisationData, O.ApiEnabled, O.ApiUser " +
                    " FROM [Organisations] O " +
                    "JOIN OrganisationStandard  OS ON OS.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "LEFT OUTER JOIN [Contacts] C ON C.Username = O.PrimaryContact AND C.EndPointAssessorOrganisationId = O.EndPointAssessorOrganisationId " +
                    "WHERE OS.StandardCode = @standardId";

            return await _unitOfWork.Connection.QueryAsync<EpaOrganisation>(sql, new { standardId });
        }

        /// <summary>
        /// This method doesn't restrict by Effective From / To and Status as the goal is return all standards
        /// of all states, specifically for use by the admin side of the service.
        /// /// </summary>
        /// <param name="organisationId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<OrganisationStandardSummary>> GetAllOrganisationStandardByOrganisationId(string organisationId)
        {
            IEnumerable<OrganisationStandardSummary> organisationStandardSummaries;

            var query =
                @"SELECT 
	                os.Id, EndPointAssessorOrganisationId AS OrganisationId, StandardCode, EffectiveFrom, EffectiveTo, DateStandardApprovedOnRegister, Comments, ContactId, OrganisationStandardData, StandardReference
                  FROM 
	                [OrganisationStandard] os 
                  WHERE 
	                EndPointAssessorOrganisationId = @organisationId

                  SELECT StandardCode, osda.DeliveryAreaId 
                  FROM 
	                [OrganisationStandard] os 
	                INNER JOIN [OrganisationStandardDeliveryArea] osda on os.Id = osda.OrganisationStandardId 
                  WHERE
	                EndPointAssessorOrganisationId = @organisationId 
                  
                  SELECT 
	                osv.StandardUId, os.StandardCode as LarsCode, s.Title, s.Level, s.IFateReferenceNumber, s.Version, osv.EffectiveFrom, osv.EffectiveTo, osv.DateVersionApproved, osv.Status, s.VersionMajor, s.VersionMinor
                  FROM 
	                [dbo].[OrganisationStandardVersion] osv
	                INNER JOIN [dbo].[OrganisationStandard] os on osv.OrganisationStandardId = os.Id
	                INNER JOIN [dbo].[Organisations] o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId 
                        AND o.EndPointAssessorOrganisationId = @organisationId
	                INNER JOIN [dbo].[Standards] s on osv.StandardUId = s.StandardUId
                  ORDER BY LarsCode, VersionMajor, VersionMinor";

            using (var multi = await _unitOfWork.Connection.QueryMultipleAsync(query, new { organisationId }))
            {
                organisationStandardSummaries = multi.Read<OrganisationStandardSummary>();
                var deliveryAreas = multi.Read().Select(a => new { a.StandardCode, a.DeliveryAreaId })?.GroupBy(a => a.StandardCode);
                var organisationStandardVersions = multi.Read<OrganisationStandardVersion>()?.GroupBy(a => a.LarsCode);

                foreach (var organisationStandardSummary in organisationStandardSummaries)
                {
                    organisationStandardSummary.DeliveryAreas = deliveryAreas?
                        .SingleOrDefault(a => a.Key == organisationStandardSummary.StandardCode)?
                        .Select(a => (int)a.DeliveryAreaId)
                        .ToList() ?? new List<int>();

                    organisationStandardSummary.StandardVersions = organisationStandardVersions?
                        .SingleOrDefault(a => a.Key == organisationStandardSummary.StandardCode)?
                        .ToList() ?? new List<OrganisationStandardVersion>();
                }
            }

            return organisationStandardSummaries;
        }

        public async Task<OrganisationStandard> GetOrganisationStandardFromOrganisationStandardId(int organisationStandardId)
        {
            OrganisationStandard orgStandard;

            var sql =
                @"SELECT Id, EndPointAssessorOrganisationId as OrganisationId, StandardCode as StandardId, StandardReference as IFateReferenceNumber, 
                             EffectiveFrom, EffectiveTo, DateStandardApprovedOnRegister, Comments, Status, ContactId, OrganisationStandardData 
                FROM [OrganisationStandard] WHERE Id = @organisationStandardId

                SELECT osv.StandardUId, os.StandardCode as LarsCode, s.Title, s.Level, s.IFateReferenceNumber, s.Version, 
                       osv.EffectiveFrom, osv.EffectiveTo, osv.DateVersionApproved, osv.Status, s.VersionMajor, s.VersionMinor
                FROM [dbo].[OrganisationStandardVersion] osv 
                INNER JOIN [dbo].[OrganisationStandard] os on osv.OrganisationStandardId = os.Id
                INNER JOIN [dbo].[Standards] s on osv.StandardUId = s.StandardUId
                WHERE osv.OrganisationStandardId = @organisationStandardId";

            using (var multi = await _unitOfWork.Connection.QueryMultipleAsync(sql, new { organisationStandardId }))
            {
                orgStandard = await multi.ReadSingleAsync<OrganisationStandard>();
                var standardVersions = multi.Read<OrganisationStandardVersion>();

                orgStandard.Versions = standardVersions?.ToList();
            }

            return orgStandard;
        }



        public async Task<IEnumerable<AppliedStandardVersion>> GetAppliedStandardVersionsForEPAO(string organisationId, string standardReference)
        {
            var sql =
                @"WITH VersionApply AS
                    (--Apply records for specific versions
                        SELECT ab1.*, og1.EndPointAssessorOrganisationId FROM(
                        SELECT ap1.Id ApplyId, ap1.ApplicationStatus, ap1.DeletedAt, ap1.OrganisationId, StandardReference, StandardReference + '_' + TRIM(version) StandardUId, ap1.ApplyData FROM Apply ap1
                        CROSS APPLY OPENJSON(ApplyData, '$.Apply.Versions') WITH(version VARCHAR(10) '$')
                        CROSS APPLY OPENJSON(ApplyData,'$.Sequences') WITH (SequenceNo INT, NotRequired BIT) sequence
                        WHERE 1=1
                          AND sequence.NotRequired = 0
                          AND sequence.sequenceNo = [dbo].[ApplyConst_STANDARD_SEQUENCE_NO]() 
                        ) ab1
                        JOIN Organisations og1 on og1.id = ab1.OrganisationId
                        WHERE ab1.standardreference IS NOT NULL
						  AND og1.EndPointAssessorOrganisationId = @organisationId
                          AND ab1.ApplicationStatus NOT IN('Approved', 'Declined')
                          AND ab1.DeletedAt IS NULL
                    )
                    --main query
                    SELECT
                        CASE 
                            WHEN NOT (os1.status = 'Live' AND (os1.EffectiveTo IS NULL OR os1.EffectiveTo > GETDATE())) THEN 'Withdrawn'
		                    WHEN osv.StandardUId IS NOT NULL 
                            THEN (CASE WHEN osv.status = 'Live' AND (osv.EffectiveTo IS NULL OR osv.EffectiveTo > GETDATE()) THEN 'Approved' ELSE 'Withdrawn' END)
		                    WHEN va1.StandardUId IS NOT NULL 
		                    THEN (CASE WHEN ApplicationStatus = 'FeedbackAdded' THEN 'Feedback Added' ELSE 'Apply in progress' END)
		                    ELSE 'Not yet applied'
		                END ApprovedStatus,
                        va1.ApplyId AS ApplicationId,
                        va1.ApplicationStatus,
                        so1.StandardUId, so1.title, so1.EffectiveFrom LarsEffectiveFrom, so1.EffectiveTo LarsEffectiveTo, so1.IFateReferenceNumber, so1.VersionEarliestStartDate, so1.VersionLatestStartDate, so1.VersionLatestEndDate, 
                        so1.version, so1.level,so1.status , so1.EPAChanged, so1.StandardPageUrl, so1.LarsCode,
                        os1.EffectiveFrom StdEffectiveFrom, os1.EffectiveTo StdEffectiveTo,
                        osv.EffectiveFrom StdVersionEffectiveFrom, osv.EffectiveTo StdVersionEffectiveTo,
                        va1.ApplyData
                        FROM standards so1 
                        LEFT JOIN organisationstandard os1 on so1.IFateReferenceNumber = os1.StandardReference AND os1.EndPointAssessorOrganisationId = @organisationId
						LEFT JOIN OrganisationStandardVersion osv on osv.standardUid = so1.standardUid AND osv.OrganisationStandardId = os1.Id 
                        LEFT JOIN VersionApply va1 on va1.StandardUId = so1.StandardUId
                        WHERE
                            so1.IFateReferenceNumber = @standardReference  
                        ORDER BY so1.Version;";

            return await _unitOfWork.Connection.QueryAsync<AppliedStandardVersion>(
                    sql, new { organisationId = organisationId, standardReference = standardReference });
        }

        public async Task<IEnumerable<OrganisationStandardPeriod>> GetOrganisationStandardPeriodsByOrganisationStandard(string organisationId, int standardId)
        {
            var sql =
                "SELECT EffectiveFrom, EffectiveTo " +
                    "FROM [OrganisationStandard] WHERE EndPointAssessorOrganisationId = @organisationId and StandardCode = @standardId";

            return await _unitOfWork.Connection.QueryAsync<OrganisationStandardPeriod>(sql, new { organisationId, standardId });
        }

        public async Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsByUkprn(string ukprn)
        {
            if (!int.TryParse(ukprn.Replace(" ", ""), out int ukprnNumeric))
            {
                return new List<AssessmentOrganisationSummary>();
            }

            var sql =
                  "SELECT o.EndPointAssessorOrganisationId as Id, o.EndPointAssessorName as Name, o.EndPointAssessorUkprn as ukprn, o.OrganisationData, o.Status, ot.Id as OrganisationTypeId, ot.Type as OrganisationType, c.Email as Email "
                    + "FROM [Organisations] o "
                    + "LEFT OUTER JOIN [OrganisationType] ot ON ot.Id = o.OrganisationTypeId "
                    + "LEFT OUTER JOIN [Contacts] c ON c.Username = o.PrimaryContact AND c.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId "
                    + "WHERE o.EndPointAssessorUkprn = @ukprnNumeric";

            return await _unitOfWork.Connection.QueryAsync<AssessmentOrganisationSummary>(sql, new { ukprnNumeric });
        }

        public async Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsByOrganisationId(string organisationId)
        {
            var sql =
                  "SELECT o.EndPointAssessorOrganisationId as Id, o.EndPointAssessorName as Name, o.EndPointAssessorUkprn as ukprn, o.OrganisationData, o.Status, ot.Id as OrganisationTypeId, ot.Type as OrganisationType, c.Email as Email "
                + "FROM [Organisations] o "
                + "LEFT OUTER JOIN [OrganisationType] ot ON ot.Id = o.OrganisationTypeId "
                + "LEFT OUTER JOIN [Contacts] c ON c.Username = o.PrimaryContact AND c.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId "
                + "WHERE o.EndPointAssessorOrganisationId like @organisationId";

            return await _unitOfWork.Connection.QueryAsync<AssessmentOrganisationSummary>(sql, new { organisationId = $"{organisationId.Replace(" ", "")}" });
        }

        public async Task<AssessmentOrganisationSummary> GetAssessmentOrganisationByContactEmail(string email)
        {
            var sql =
                "SELECT top 1 o.EndPointAssessorOrganisationId as Id, o.EndPointAssessorName as Name, o.EndPointAssessorUkprn as ukprn, o.OrganisationData, o.Status, ot.Id as OrganisationTypeId, ot.Type as OrganisationType, pc.Email as Email "
                    + "FROM [Organisations] o "
                    + "LEFT OUTER JOIN [OrganisationType] ot ON ot.Id = o.OrganisationTypeId "
                    + "LEFT OUTER JOIN [Contacts] pc ON pc.Username = o.PrimaryContact AND pc.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId "
                    + "LEFT JOIN [Contacts] c ON c.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId "
                    + "WHERE replace(c.Email, ' ','')  = replace(@email, ' ','')";

            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<AssessmentOrganisationSummary>(sql, new { email });
        }

        public async Task<IEnumerable<AssessmentOrganisationSummary>> GetAssessmentOrganisationsByNameOrCharityNumberOrCompanyNumber(string searchString)
        {
            var sql =
                "SELECT o.EndPointAssessorOrganisationId as Id, o.EndPointAssessorName as Name, o.EndPointAssessorUkprn as ukprn, o.OrganisationData, o.Status, ot.Id as OrganisationTypeId, ot.Type as OrganisationType, c.Email as Email "
                    + "FROM [Organisations] o "
                    + "LEFT OUTER JOIN [OrganisationType] ot ON ot.Id = o.OrganisationTypeId "
                    + "LEFT OUTER JOIN [Contacts] c ON c.Username = o.PrimaryContact AND c.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId "
                    + "WHERE replace(o.EndPointAssessorName, ' ','') like @searchString "
                    + "OR replace(JSON_VALUE(o.[OrganisationData], '$.TradingName'), ' ','') like @searchString "
                    + "OR replace(JSON_VALUE(o.[OrganisationData], '$.LegalName'), ' ','') like @searchString "
                    + "OR replace(JSON_VALUE(o.[OrganisationData], '$.CompanyNumber'), ' ','') like @searchString "
                    + "OR replace(JSON_VALUE(o.[OrganisationData], '$.CharityNumber'), ' ','') like @searchString ";

            return await _unitOfWork.Connection.QueryAsync<AssessmentOrganisationSummary>(sql, new { searchString = $"%{searchString.Replace(" ", "")}%" });
        }

        public async Task<EpaContact> GetContactByContactId(Guid contactId)
        {
            var sql =
                "select Id, EndPointAssessorOrganisationId, Username,GivenNames, DisplayName, FamilyName, SigninId, SigninType, Email, Status, PhoneNumber " +
                    " from Contacts where Id = @contactId";

            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<EpaContact>(sql, new { contactId });
        }

        public async Task<EpaContact> GetContactByEmail(string email)
        {
            var sql =
                "select Id, EndPointAssessorOrganisationId, Username,GivenNames, DisplayName, FamilyName, SigninId, SigninType, Email, Status, PhoneNumber " +
                    " from Contacts where Email = @email";

            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<EpaContact>(sql, new { email });
        }

        public async Task<EpaContact> GetContactBySignInId(string signinId)
        {
            var sql =
                "select Id, EndPointAssessorOrganisationId, Username,GivenNames, DisplayName, FamilyName, SigninId, SigninType, Email, Status, PhoneNumber " +
                    " from Contacts where SigninId = @signinId";

            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<EpaContact>(sql, new { signinId });
        }

        public async Task<IEnumerable<OrganisationStandardDeliveryArea>> GetDeliveryAreasByOrganisationStandardId(
            int organisationStandardId)
        {
            var sql =
                "select *  from organisationStandardDeliveryArea" +
                    " where OrganisationStandardId = @organisationStandardId";

            return await _unitOfWork.Connection.QueryAsync<OrganisationStandardDeliveryArea>(sql, new { organisationStandardId });
        }


        public async Task<string> GetEpaOrgIdByEndPointAssessmentName(string name)
        {
            var sql =
                    "SELECT  O.EndPointAssessorOrganisationId " +
                    " FROM [Organisations] O "
                    + "WHERE replace(o.EndPointAssessorName, ' ','') like @name "
                    + "OR replace(JSON_VALUE(o.[OrganisationData], '$.TradingName'), ' ','') like @name "
                    + "OR replace(JSON_VALUE(o.[OrganisationData], '$.LegalName'), ' ','') like @name ";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<string>(sql, new { name });
        }
    }
}