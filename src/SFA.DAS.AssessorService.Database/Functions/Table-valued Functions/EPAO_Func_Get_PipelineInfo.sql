CREATE FUNCTION [dbo].[EPAO_Func_Get_PipelineInfo]
(	
	@epaOrgId NVARCHAR(12),
	@stdCode INT,
	@pipelineCutOff INT
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
			CASE WHEN PlannedEndDate > GETDATE() THEN EOMONTH(PlannedEndDate) ELSE EOMONTH(DATEADD(month, [StandardInfo].Duration, LearnStartDate)) END EstimateDate
		FROM 
			[Ilrs]
			-- ignore standards that have been deleted, are expired or have been withdrawn
			INNER JOIN [OrganisationStandard] ON [Ilrs].EpaOrgId = [OrganisationStandard].EndPointAssessorOrganisationId 
				AND [Ilrs].StdCode = [OrganisationStandard].StandardCode 
				AND [OrganisationStandard].[Status] <> 'Deleted' 
				AND ([OrganisationStandard].[EffectiveTo] IS NULL OR [OrganisationStandard].[EffectiveTo] >= GETDATE())
			INNER JOIN 
			(
				SELECT StandardId, Title, CONVERT(numeric,JSON_VALUE([StandardData],'$.Duration')) Duration FROM [dbo].[StandardCollation]
			) [StandardInfo] ON [StandardInfo].StandardId = [Ilrs].StdCode
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
			AND 
			( 
				(
					-- remove Ilrs for funding model 36 (Apprenticeships) which are Continuing 
					-- if the most recent active submission is greater than 6 months ago
					FundingModel = 36 
					AND CompletionStatus = 1 
					AND ISNULL([Ilrs].UpdatedAt, [Ilrs].CreatedAt) >= DATEADD(month, -6, GETDATE())
				) 
				OR CompletionStatus = 2 
				OR FundingModel != 36
			) 
			-- remove Ilrs which already have certificates (by uln and standardcode)
			AND [ExistingCertificate].Uln IS NULL
	) [PipelineInfo]
	WHERE
		-- remove Ilrs for which the estimated date of completion has passed the cutoff
		EstimateDate >= DATEADD(month, -@pipelineCutOff, GETDATE())
)