CREATE FUNCTION [dbo].[OppFinder_Is_InDevelopment_StandardStatus]
(
	@StandardData VARCHAR(MAX)
)
RETURNS BIT
AS
BEGIN
	IF (ISNULL(JSON_VALUE(@StandardData, '$.IfaStatus'), '') = 'In development') 
	   AND -- if there is any value in IntegratedDegree it is not a valid InDevelopment standard
	   (ISNULL(JSON_VALUE(@StandardData, '$.IntegratedDegree'), '') = '')

	BEGIN
		RETURN 1
	END
	
	RETURN 0
END