-- This script will add data to the [CertificateBatchLogs] table this table is referenced
-- by a foreign key but the data which is reference is added in a post deployment script
-- because it is necessary for the DAC pac to have previously created the table

-- Any [Certificate].BatchNumber which does not actually refer to a [BatchLogs].BatchNumber
-- will be set to NULL - this should have been a foreign key in the first instance. It has
-- been defaulted to a non-existant batch number when there is a problem creating a batch number
-- there is no data loss by setting this back to NULL

-- Since the foreign key are created during DAC PAC deployment automatically with NOCHECK the
-- missing data will not cause a deployment failure; after post deployment is completed the DAC
-- PAC deployment will automatically enable the foreign key CHECK constraints on the new data

BEGIN TRANSACTION
	-- Remove invalid batch number from the certficate table
	SELECT 
		DISTINCT [Certificates].BatchNumber 
	INTO
		#InvalidBatchNumber
	FROM 
		[Certificates] LEFT JOIN [BatchLogs]
		ON [Certificates].BatchNumber = [BatchLogs].BatchNumber
	WHERE 
		[Certificates].BatchNumber IS NOT NULL 
		AND [BatchLogs].BatchNumber IS NULL
	
	UPDATE [Certificates] SET BatchNumber = NULL WHERE BatchNumber IN
	(
		SELECT * FROM #InvalidBatchNumber
	)

	DROP TABLE #InvalidBatchNumber

	-- Insert [CertificateBatchLogs] for each [Certificate] latest batch number
	INSERT INTO [CertificateBatchLogs] (CertificateReference, BatchNumber, CertificateData, Status, StatusAt, CreatedAt, CreatedBy)
	SELECT
		[Certificates].CertificateReference,
		[Certificates].BatchNumber,
		'{ }' CertificateData,
		'Printed' Status,
		ToBePrinted StatusAt,
		GETUTCDATE() CreatedAt,
		'Deployment' CreatedBy
	FROM 
		[Certificates] LEFT JOIN [CertificateBatchLogs]
	ON [Certificates].CertificateReference = [CertificateBatchLogs].CertificateReference AND [Certificates].BatchNumber = [CertificateBatchLogs].BatchNumber
	WHERE 
		[Certificates].ToBePrinted IS NOT NULL AND [Certificates].BatchNumber IS NOT NULL
		-- only add the data if it is currently missing
		AND [CertificateBatchLogs].CertificateReference IS NULL AND [CertificateBatchLogs].BatchNumber IS NULL
COMMIT TRANSACTION

