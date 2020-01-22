CREATE FUNCTION [dbo].[OppFinder_Is_Proposed_StandardStatus]
(
	@StandardData VARCHAR(MAX)
)
RETURNS BIT
AS
BEGIN
	IF (ISNULL(JSON_VALUE(@StandardData, '$.IfaStatus'), '') = 'Proposal in development') 
	   AND -- standards which are integrated degrees are not valid proposed standards
	   (ISNULL(JSON_VALUE(@StandardData, '$.IntegratedDegree'), '') <> 'integrated degree')
	BEGIN
		RETURN 1
	END
	
	RETURN 0
END