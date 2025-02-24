CREATE TABLE [frameworks].[maig_certificates]
(
  [cert_id] bigint NOT NULL,
  [cert_description] nvarchar(400),
  [cert_details] nvarchar(400),
  [cert_file] nvarchar(200),
  [cert_code] nvarchar(max),
  PRIMARY KEY ([cert_id])
)
GO