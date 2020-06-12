CREATE FUNCTION [dbo].[OppFinder_Is_Proposed_StandardStatus]
(
	@StandardData VARCHAR(MAX)
)
RETURNS BIT
AS
BEGIN
	IF (ISNULL(JSON_VALUE(@StandardData, '$.IfaStatus'), '') = 'Proposal in development') 
	   AND -- standards which have exactly the text 'integrated degree' are not valid proposed standards in opp finder
	   (ISNULL(JSON_VALUE(@StandardData, '$.IntegratedDegree'), '') <> 'integrated degree')
	BEGIN
		RETURN 1
	END
	
	RETURN 0
END