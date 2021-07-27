CREATE FUNCTION [dbo].[ExpandedVersion]
(
	@StandardVersion VARCHAR(10)
)
RETURNS VARCHAR(10)
AS
BEGIN
	RETURN  RIGHT('00000'+REPLACE(LEFT(@StandardVersion,PATINDEX('%.%',@StandardVersion)),'.',''),5) +  RIGHT('00000'+REPLACE(@StandardVersion,LEFT(@StandardVersion,PATINDEX('%.%',@StandardVersion)),''),5)
END
GO