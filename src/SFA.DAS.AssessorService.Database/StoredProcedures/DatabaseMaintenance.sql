CREATE PROCEDURE [dbo].[DatabaseMaintenance]
AS
BEGIN
	DECLARE @Start DATETIME = GETUTCDATE()
	DECLARE @Results TABLE(Step VARCHAR(100))

	ALTER INDEX ALL ON [dbo].[Certificates]
	REBUILD WITH
	(
	    FILLFACTOR = 80,
	    ONLINE = ON
	);

	INSERT @Results VALUES('Rebuild ALL Indexes [Certificates] : ' + CONVERT(VARCHAR, DATEDIFF(minute, @Start, GETUTCDATE())) + ' minutes')

	SET @Start = GETUTCDATE()

	ALTER INDEX ALL ON [dbo].[CertificateLogs]
	REBUILD WITH
	(
	    FILLFACTOR = 80,
	    ONLINE = ON
	);

	INSERT @Results VALUES('Rebuild ALL Indexes [CertificateLogs] : ' + CONVERT(VARCHAR, DATEDIFF(minute, @Start, GETUTCDATE())) + ' minutes')

	SET @Start = GETUTCDATE()

	ALTER INDEX ALL ON [dbo].[Ilrs]
	REBUILD WITH
	(
	    FILLFACTOR = 80,
	    ONLINE = ON
	);

	INSERT @Results VALUES('Rebuild ALL Indexes [Ilrs] : ' + CONVERT(VARCHAR, DATEDIFF(minute, @Start, GETUTCDATE())) + ' minutes')

	SELECT * FROM @Results
END
