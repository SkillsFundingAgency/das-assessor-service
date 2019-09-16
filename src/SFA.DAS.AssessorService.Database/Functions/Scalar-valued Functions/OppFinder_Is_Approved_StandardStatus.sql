CREATE FUNCTION [dbo].[OppFinder_Is_Approved_StandardStatus]
(
	@StandardData VARCHAR(MAX)
)
RETURNS BIT
AS
BEGIN
	IF (ISNULL(JSON_VALUE(@StandardData, '$.IfaStatus'), '') = 'Approved for delivery')
	   AND -- if there is any value in IntegratedDegree it is not a valid Approved standard
	   (ISNULL(JSON_VALUE(@StandardData, '$.IntegratedDegree'), '') = '')
	BEGIN
		RETURN 1
	END
	
	RETURN 0
END