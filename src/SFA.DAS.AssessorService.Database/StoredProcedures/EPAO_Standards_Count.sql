CREATE PROCEDURE [dbo].[EPAO_Standards_Count] 
@EPAO NVARCHAR(12),
@Count int out
AS

BEGIN

SELECT @Count = COUNT(*)
FROM OrganisationStandard os
WHERE EXISTS (SELECT 1
              FROM OrganisationStandardVersion osv
              WHERE os.Id = osv.OrganisationStandardId 
					AND osv.Status = 'Live' 
					AND (osv.EffectiveTo is null OR osv.EffectiveTo > GETDATE()))
	AND EndPointAssessorOrganisationId = @EPAO
	AND os.Status = 'Live' 
	AND (os.EffectiveTo is null OR os.EffectiveTo > GETDATE())

END
GO