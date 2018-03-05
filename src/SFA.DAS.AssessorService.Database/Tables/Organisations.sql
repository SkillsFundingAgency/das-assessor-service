CREATE TABLE [dbo].[Organisations](
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
	[EndPointAssessorName] [nvarchar](max) NULL,
	[EndPointAssessorOrganisationId] [nvarchar](max) NULL,
	[EndPointAssessorUkprn] [int] NOT NULL,
	[PrimaryContactId] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[Status] [nvarchar](max) NULL,
 CONSTRAINT [PK_Organisations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]