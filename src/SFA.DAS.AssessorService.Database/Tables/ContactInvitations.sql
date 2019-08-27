CREATE TABLE [dbo].[ContactInvitations](
    [Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[InvitationDate] [datetime2](7) NOT NULL,
	[InvitorContactId] [uniqueidentifier] NOT NULL,
	[InviteeContactId] [uniqueidentifier] NOT NULL,
	[OrganisationId] [uniqueidentifier] NOT NULL,
	[IsAccountCreated] [bit] NOT NULL DEFAULT 0,
	[AccountCreatedDate] [datetime2](7) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE INDEX [IXU_ContactInvitations] ON [ContactInvitations] ([InviteeContactId])
GO