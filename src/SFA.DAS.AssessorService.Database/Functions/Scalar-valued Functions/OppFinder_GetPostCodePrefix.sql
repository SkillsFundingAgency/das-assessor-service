CREATE FUNCTION [dbo].[OppFinder_GetPostCodePrefix]
(
	@PostCode VARCHAR(MAX)
)
RETURNS NVARCHAR(2)
AS
BEGIN
    DECLARE @End_Pos INT = 0
	DECLARE @PostCodePrefix nvarchar(2) = ''
    set @End_Pos = PATINDEX('%[0-9]%',ISNULL(@PostCode,'ZZ'))

	IF (@End_Pos > 0) 
        set @PostCodePrefix = LEFT(@PostCode, (@End_Pos - 1))
	
	return @PostCodePrefix
END