-- Data for GET /api/ao/assessments?standard={IfateReferenceNumber}&ukprn={ukprn}
--
CREATE PROCEDURE [dbo].[AssessmentsSummaryUpdate]
AS

SET NOCOUNT ON;

DECLARE @Error_Code INT = 0
		,@Error_Message VARCHAR(MAX)
		,@Error_Severity INT = 0

BEGIN
	BEGIN TRANSACTION T1;
	BEGIN TRY;
	
	MERGE INTO [dbo].[AssessmentsSummary] cps
	USING
	(
		SELECT [ProviderUkPrn] Ukprn ,[StandardReference] IfateReferenceNumber
			   ,MIN([AchievementDate]) EarliestAssessment
			   ,COUNT(*) EndpointAssessmentCount
		FROM [dbo].[StandardCertificates] 
		WHERE [IsPrivatelyFunded] = 0
		AND [Status] NOT IN ('Deleted','Draft')
	GROUP BY [ProviderUkPrn],[StandardReference]
	) upd 
	ON (cps.[Ukprn] = upd.[Ukprn] AND cps.[IfateReferenceNumber] = upd.[IfateReferenceNumber])
	WHEN MATCHED 
	AND (cps.[EarliestAssessment] != upd.[EarliestAssessment] OR cps.[EndpointAssessmentCount] != upd.[EndpointAssessmentCount] )
	THEN UPDATE 
		 SET cps.[EarliestAssessment] = upd.[EarliestAssessment] 
	        ,cps.[EndpointAssessmentCount] = upd.[EndpointAssessmentCount]
			,cps.[UpdatedAt] = GETUTCDATE()
	WHEN NOT MATCHED BY TARGET
	THEN INSERT ([Ukprn], [IfateReferenceNumber], [EarliestAssessment] , [EndpointAssessmentCount], [UpdatedAt])
		 VALUES (upd.[Ukprn], upd.[IfateReferenceNumber], upd.[EarliestAssessment] , upd.[EndpointAssessmentCount], GETUTCDATE())
	WHEN NOT MATCHED BY SOURCE
	THEN DELETE
	;
	
	SELECT @@ROWCOUNT;

	COMMIT TRANSACTION T1;
  
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION T1;

		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
		DECLARE @ErrorState INT = ERROR_STATE();

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH

END