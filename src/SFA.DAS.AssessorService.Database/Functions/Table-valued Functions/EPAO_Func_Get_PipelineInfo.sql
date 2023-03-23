-- NOTE limits to filter out of date learner records
-- Allow 6 months over-run on lapsed learner data updates
-- Allow configurable months to overrun beyond estimated end date
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
		pv1.Ukprn,
		StdCode,
		Title,
		EstimatedEndDate EstimateDate,
		pv1.Name ProviderName,
		Version
	FROM
	(

		--Get most up to date data from Learner and Ilrs tables
		SELECT le2.UkPrn, 
			   le2.StdCode, 
			   le2.StandardReference, 
			   le2.StandardName Title, 
			   le2.Version, 
			   le2.LastUpdated, 
			   le2.EstimatedEndDate
		FROM 
			Learner le2
		INNER JOIN 
			-- must have at least one live version of a standard for the given EPAO
			   (SELECT DISTINCT StandardCode FROM OrganisationStandard os1
				  JOIN OrganisationStandardVersion osv ON osv.OrganisationStandardId = os1.Id
				 WHERE os1.Status = 'Live' 
					AND (os1.EffectiveTo IS NULL OR os1.EffectiveTo >= GETDATE())
					AND osv.Status = 'Live' 
					AND (osv.EffectiveTo IS NULL OR osv.EffectiveTo >= GETDATE())
					-- only standnard data for the given EPAO
					AND os1.EndPointAssessorOrganisationId = @epaOrgId
					-- filter by standard code if required
					AND ( os1.StandardCode = @stdCode OR @stdCode IS NULL ) 
				) os2 on os2.Standardcode = le2.StdCode
		LEFT JOIN 
			( SELECT DISTINCT Uln, StandardCode FROM Certificates ) [ExistingCertificate] ON [ExistingCertificate].Uln = le2.Uln AND [ExistingCertificate].StandardCode = le2.StdCode
		WHERE 1=1
			-- exclude already created certificates (by uln and standardcode)
			AND [ExistingCertificate].Uln IS NULL	  -- only include learner data for the given EPAO which is Active, or completed
			AND EpaOrgId = @epaOrgId
			-- and for continuing or recently completed Apprenticeships
			AND CompletionStatus IN (1,2)
			-- limit pipeline to completed, or continuing learning that has not yet lapsed
			AND (
				-- Learner has completed 
				CompletionStatus = 2 
				-- most recent activity (approval/ILR submission) is no more than 6(?) months ago
				OR (CompletionStatus = 1 AND LastUpdated >= DATEADD(month, -6, GETDATE()) )
				)
			-- limit Pipeline to the Estimated End Date is no more than the configurable pipeline cut off.
			AND EstimatedEndDate >= DATEADD(month, -@pipelineCutOff, GETDATE())
		) [PipelineInfo]
		INNER JOIN Providers pv1 ON pv1.Ukprn = [PipelineInfo].UkPrn 

 )
