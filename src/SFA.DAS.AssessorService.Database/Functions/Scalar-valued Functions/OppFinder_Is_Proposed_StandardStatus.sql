CREATE FUNCTION [dbo].[OppFinder_Is_Proposed_StandardStatus]
(
	@StandardData VARCHAR(MAX)
)
RETURNS BIT
AS
BEGIN
	IF (ISNULL(JSON_VALUE(@StandardData, '$.IfaStatus'), '') = 'Proposal in development') 
	   AND -- if there is any value in IntegratedDegree it is not a valid proposed standard
	   (ISNULL(JSON_VALUE(@StandardData, '$.IntegratedDegree'), '') = '')
	BEGIN
		RETURN 1
	END
	
	RETURN 0
END