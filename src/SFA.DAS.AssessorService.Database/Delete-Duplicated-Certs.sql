-- Remove Duplicated Certificates ON-2222
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'CertificatesDeleted'))
BEGIN
    CREATE TABLE [CertificatesDeleted](
    [Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
    [CertificateData] [nvarchar](max) NOT NULL,
    [ToBePrinted] [datetime2](7) NULL,
    [CreatedAt] [datetime2](7) NOT NULL,
    [CreatedBy] [nvarchar](256) NOT NULL,
    [DeletedAt] [datetime2](7) NULL,
    [DeletedBy] [nvarchar](256) NULL,
    [CertificateReference] VARCHAR(50) NOT NULL,
    [OrganisationId] [uniqueidentifier] NOT NULL,
    [BatchNumber] [int] NULL,
    [Status] [nvarchar](20) NOT NULL,
    [UpdatedAt] [datetime2](7) NULL,
    [UpdatedBy] [nvarchar](256) NULL, 
    [Uln] BIGINT NOT NULL, 
    [StandardCode] INT NOT NULL, 
    [ProviderUkPrn] INT NOT NULL, 
    [CertificateReferenceId] INT NOT NULL, 
    [LearnRefNumber] NVARCHAR(12) NULL,
    [CreateDay] DATE NOT NULL,
    [IsPrivatelyFunded] BIT, 
    [PrivatelyFundedStatus] NVARCHAR(20) NULL
    )
    
    CREATE TABLE [CertificateLogsDeleted](
    [Id] [uniqueidentifier] NOT NULL,
    [Action] [nvarchar](400) NULL,
    [CertificateId] [uniqueidentifier] NOT NULL,
    [EventTime] [datetime2](7) NOT NULL,
    [Status] [nvarchar](20) NOT NULL,
    [CertificateData] NVARCHAR(MAX) NOT NULL, 
    [Username] NVARCHAR(256) NOT NULL,
    [BatchNumber] [int] NULL,
    [ReasonForChange] NVARCHAR(MAX) NULL
    )
    
    INSERT INTO [CertificatesDeleted]
    select ce1.* from Certificates ce1
    join (
    Select ID, ROW_NUMBER() OVER (PARTITION BY uln, StandardCode ORDER BY CASE WHEN Status IN ('Draft','Deleted') THEN 1 ELSE 0 END, Certificatereferenceid DESC) rownumber From Certificates
    ) ab1 ON ab1.id = ce1.id
    WHERE rownumber=2
    
    INSERT INTO [CertificateLogsDeleted]
    SELECT * FROM CertificateLogs WHERE CertificateID IN (SELECT ID from CertificatesDeleted)
    
    DELETE FROM CertificateLogs WHERE CertificateID IN (SELECT ID from CertificatesDeleted)
    
    DELETE FROM Certificates WHERE ID IN (SELECT ID from CertificatesDeleted)
    
END
GO

