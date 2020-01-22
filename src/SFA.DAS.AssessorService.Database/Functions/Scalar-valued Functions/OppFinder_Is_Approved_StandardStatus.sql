CREATE FUNCTION [dbo].[OppFinder_Is_Approved_StandardStatus]
(
	@StandardData VARCHAR(MAX)
)
RETURNS BIT
AS
BEGIN
	IF (ISNULL(JSON_VALUE(@StandardData, '$.IfaStatus'), '') = 'Approved for delivery')
	   AND -- standards which are integrated degrees are not valid approved standards
	   (ISNULL(JSON_VALUE(@StandardData, '$.IntegratedDegree'), '') <> 'integrated degree')
	BEGIN
		RETURN 1
	END
	
	RETURN 0
END