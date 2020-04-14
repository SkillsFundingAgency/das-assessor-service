CREATE FUNCTION [dbo].[ToTicks]
(
	@DateTime datetime
)
  RETURNS bigint
AS
BEGIN
    RETURN DATEDIFF_BIG( microsecond, '00010101', @DateTime ) * 10 + ( DATEPART( NANOSECOND, @DateTime ) % 1000 ) / 100;
END
