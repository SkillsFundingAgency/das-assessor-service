CREATE FUNCTION [dbo].[CleanseName] 
(
@name NVARCHAR(max)
)
RETURNS NVARCHAR(max)
WITH SCHEMABINDING
AS
BEGIN
-- remove non-printing characters, back ticks etc from name fields
RETURN 
TRIM(REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(TRIM(REPLACE(REPLACE(REPLACE(
TRANSLATE(@name,'‘`’–_=\|~'+NCHAR(0x200B)+NCHAR(0x00A0)+NCHAR(0x0009),'''''''---####  ')	
,'#',''),'  ',' '),'O?','O''')),','''),'''-'),'  ',' '),' -','-'),'- ','-'))
END;