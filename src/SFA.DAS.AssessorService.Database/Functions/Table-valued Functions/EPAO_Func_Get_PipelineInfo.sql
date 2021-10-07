CREATE FUNCTION [dbo].[EPAO_Func_Get_PipelineInfo]
(	
	@epaOrgId NVARCHAR(12),
	@stdCode INT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		UkPrn,
		StdCode,
		Title,
		EstimateDate
	FROM
	(
		SELECT 
			[Ilrs].UkPrn,
			[Ilrs].StdCode,
			[StandardInfo].Title,
			CASE WHEN PlannedEndDate > GETDATE() THEN EOMONTH(PlannedEndDate) ELSE EOMONTH(DATEADD(month, [StandardInfo].TypicalDuration, LearnStartDate)) END EstimateDate
		FROM 
			[Ilrs]
			-- ignore standards that have been deleted, are expired or have been withdrawn
			INNER JOIN [OrganisationStandard] ON [Ilrs].EpaOrgId = [OrganisationStandard].EndPointAssessorOrganisationId 
				AND [Ilrs].StdCode = [OrganisationStandard].StandardCode 
				AND [OrganisationStandard].[Status] <> 'Deleted' 
				AND ([OrganisationStandard].[EffectiveTo] IS NULL OR [OrganisationStandard].[EffectiveTo] >= GETDATE())
			INNER JOIN 
			(
				SELECT LarsCode, Title, TypicalDuration FROM [dbo].[Standards]
			) [StandardInfo] ON [StandardInfo].LarsCode = [Ilrs].StdCode
			LEFT JOIN 
			(
				SELECT DISTINCT Uln, StandardCode FROM [Certificates] c1 
			) [ExistingCertificate] ON [ExistingCertificate].Uln = [Ilrs].Uln AND [ExistingCertificate].StandardCode = [Ilrs].StdCode
		WHERE 
			CompletionStatus IN (1,2) 
			AND EpaOrgId = @epaOrgId
			AND 
			(
				[OrganisationStandard].StandardCode = @stdCode OR @stdCode IS NULL
			)
			-- limit Pipeline to where Ilr is completed or most recent active submission is no more than 6 months ago
			AND 
			( 
				(
					FundingModel = 36 
					AND CompletionStatus = 1 
					AND ISNULL([Ilrs].UpdatedAt, [Ilrs].CreatedAt) >= DATEADD(month, -6, GETDATE())
				) 
				OR CompletionStatus = 2 
				OR FundingModel != 36
			) 
			-- exclude already created certificates (by uln and standardcode)
			AND [ExistingCertificate].Uln IS NULL
	) [PipelineInfo]
	WHERE
		EstimateDate >= DATEADD(month,-3,GETDATE())
)