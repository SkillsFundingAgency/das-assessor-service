CREATE TABLE [DeliveryArea](
	[Id] [int] IDENTITY (1,1) PRIMARY KEY,
	[Area] [nvarchar](256) NOT NULL, 
    [Status] [nvarchar](10) NOT NULL, 
    [Ordering] INT NOT NULL DEFAULT 0,
) ON [PRIMARY] 
