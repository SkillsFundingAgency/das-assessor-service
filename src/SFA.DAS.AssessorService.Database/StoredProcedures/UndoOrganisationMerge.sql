CREATE PROCEDURE [dbo].[UndoOrganisationMerge]
	@mergeOrganisationId INT,
    @updatedBy NVARCHAR(256)
AS


-- Validate that the merge can be restored

BEGIN TRY
IF NOT EXISTS (SELECT * FROM MergeOrganisations WHERE Id = @mergeOrganisationId AND Status = 'Completed') RAISERROR('Unable to undo merge', 16, 1)
END TRY
BEGIN CATCH
RETURN
END CATCH


DECLARE @PrimaryEndPointAssessorOrganisationId NVARCHAR(12)
SET @PrimaryEndPointAssessorOrganisationId = (SELECT PrimaryEndPointAssessorOrganisationId FROM MergeOrganisations WHERE Id = @mergeOrganisationId)

DECLARE @SecondaryEndPointAssessorOrganisationId NVARCHAR(12)
SET @SecondaryEndPointAssessorOrganisationId = (SELECT SecondaryEndPointAssessorOrganisationId FROM MergeOrganisations WHERE Id = @mergeOrganisationId)

BEGIN TRANSACTION


-- Undelete any Applications

DROP TABLE IF EXISTS #MergeApplyBefore
SELECT * INTO #MergeApplyBefore FROM MergeApply WHERE MergeOrganisationId = @mergeOrganisationId AND Replicates = 'Before'

MERGE [Apply] [Target] USING #MergeApplyBefore [Source]
ON ([Source].ApplyId = [Target].Id)
WHEN MATCHED
    THEN UPDATE SET 
        [Target].[ApplicationId] = [Source].[ApplicationId],
        [Target].[OrganisationId] = [Source].[OrganisationId],
        [Target].[ApplicationStatus] = [Source].[ApplicationStatus],
        [Target].[ReviewStatus] = [Source].[ReviewStatus],
        [Target].[ApplyData] = [Source].[ApplyData],
        [Target].[FinancialReviewStatus] = [Source].[FinancialReviewStatus],
        [Target].[FinancialGrade] = [Source].[FinancialGrade],
        [Target].[StandardCode] = [Source].[StandardCode],
        [Target].[CreatedAt] = [Source].[CreatedAt],
        [Target].[CreatedBy] = [Source].[CreatedBy],
        [Target].[UpdatedAt] = [Source].[UpdatedAt],
        [Target].[UpdatedBy] = [Source].[UpdatedBy],
        [Target].[DeletedAt] = [Source].[DeletedAt],
        [Target].[DeletedBy] = [Source].[DeletedBy],
        [Target].[StandardApplicationType] = [Source].[StandardApplicationType],
        [Target].[StandardReference] = [Source].[StandardReference]

WHEN NOT MATCHED BY TARGET 
    THEN INSERT ([Id], 
    [ApplicationId], 
    [OrganisationId], 
    [ApplicationStatus], 
    [ReviewStatus], 
    [ApplyData],
    [FinancialReviewStatus],
    [FinancialGrade],
    [StandardCode], [CreatedAt],[CreatedBy],[UpdatedAt],[UpdatedBy],[DeletedAt],[DeletedBy],[StandardApplicationType],[StandardReference]
    )
         VALUES ([Source].[ApplyId], [Source].[ApplicationId], [Source].[OrganisationId], [Source].[ApplicationStatus], [Source].[ReviewStatus],
         [Source].[ApplyData],[Source].[FinancialReviewStatus],[Source].[FinancialGrade],[Source].[StandardCode],[Source].[CreatedAt],[Source].[CreatedBy],
         [Source].[UpdatedAt],[Source].[UpdatedBy],[Source].[DeletedAt],[Source].[DeletedBy],[Source].[StandardApplicationType],[Source].[StandardReference]
         );


-- Use the snapshots to delete the deltas of added data

DELETE FROM OrganisationStandardDeliveryArea WHERE Id IN (SELECT OrganisationStandardDeliveryAreaId FROM MergeOrganisationStandardDeliveryArea WHERE MergeOrganisationId = @mergeOrganisationId AND Replicates = 'After' AND OrganisationStandardDeliveryAreaId NOT IN (SELECT OrganisationStandardDeliveryAreaId FROM MergeOrganisationStandardDeliveryArea WHERE MergeOrganisationId = @mergeOrganisationId AND Replicates = 'Before'))
DELETE FROM OrganisationStandardVersion WHERE OrganisationStandardId IN (SELECT OrganisationStandardId FROM MergeOrganisationStandardVersion WHERE MergeOrganisationId = @mergeOrganisationId AND Replicates = 'After' AND OrganisationStandardId NOT IN (SELECT OrganisationStandardId FROM MergeOrganisationStandardVersion WHERE MergeOrganisationId = @mergeOrganisationId AND Replicates = 'Before'))
DELETE FROM OrganisationStandard WHERE Id IN (SELECT OrganisationStandardId FROM MergeOrganisationStandard WHERE MergeOrganisationId = @mergeOrganisationId AND Replicates = 'After' AND OrganisationStandardId NOT IN (SELECT OrganisationStandardId FROM MergeOrganisationStandard WHERE MergeOrganisationId = @mergeOrganisationId AND Replicates = 'Before'))


-- Revert OrganisationStandard to the "Before" data

DROP TABLE IF EXISTS #MergeOrganisationStandardBefore
SELECT * INTO #MergeOrganisationStandardBefore FROM MergeOrganisationStandard WHERE MergeOrganisationId = @mergeOrganisationId AND Replicates = 'Before'

SET IDENTITY_INSERT [OrganisationStandard] ON

MERGE [OrganisationStandard] [Target] USING #MergeOrganisationStandardBefore [Source]
ON ([Source].OrganisationStandardId = [Target].Id)
WHEN MATCHED
    THEN UPDATE SET 
        [Target].[EndPointAssessorOrganisationId] = [Source].[EndPointAssessorOrganisationId],
        [Target].[StandardCode] = [Source].[StandardCode],
        [Target].[EffectiveFrom] = [Source].[EffectiveFrom],
        [Target].[EffectiveTo] = [Source].[EffectiveTo],
        [Target].[DateStandardApprovedOnRegister] = [Source].[DateStandardApprovedOnRegister],
        [Target].[Comments] = [Source].[Comments],
        [Target].[Status] = [Source].[Status],
        [Target].[ContactId] = [Source].[ContactId],
        [Target].[OrganisationStandardData] = [Source].[OrganisationStandardData],
        [Target].[StandardReference] = [Source].[StandardReference]

WHEN NOT MATCHED BY TARGET 
    THEN INSERT ([Id], 
        [EndPointAssessorOrganisationId],
        [StandardCode],
        [EffectiveFrom],
        [EffectiveTo],
        [DateStandardApprovedOnRegister],
        [Comments],
        [Status],
        [ContactId],
        [OrganisationStandardData],
        [StandardReference]
    )
         VALUES (
         [Source].[OrganisationStandardId], 
        [Source].[EndPointAssessorOrganisationId],
        [Source].[StandardCode],
        [Source].[EffectiveFrom],
        [Source].[EffectiveTo],
        [Source].[DateStandardApprovedOnRegister],
        [Source].[Comments],
        [Source].[Status],
        [Source].[ContactId],
        [Source].[OrganisationStandardData],
        [Source].[StandardReference]
         );

SET IDENTITY_INSERT [OrganisationStandard] OFF


-- Revert OrganisationStandardVersion to the "Before" data

DROP TABLE IF EXISTS #MergeOrganisationStandardVersionBefore
SELECT * INTO #MergeOrganisationStandardVersionBefore FROM MergeOrganisationStandardVersion WHERE MergeOrganisationId = @mergeOrganisationId AND Replicates = 'Before'

MERGE [OrganisationStandardVersion] [Target] USING #MergeOrganisationStandardVersionBefore [Source]
ON ([Source].OrganisationStandardId = [Target].OrganisationStandardId AND [Source].StandardUId = [Target].StandardUId)
WHEN MATCHED
    THEN UPDATE SET 
        [Target].[Version] = [Source].[Version],
        [Target].[EffectiveFrom] = [Source].[EffectiveFrom],
        [Target].[EffectiveTo] = [Source].[EffectiveTo],
        [Target].[DateVersionApproved] = [Source].[DateVersionApproved],
        [Target].[Comments] = [Source].[Comments],
        [Target].[Status] = [Source].[Status]

WHEN NOT MATCHED BY TARGET 
    THEN INSERT (
    [OrganisationStandardId], 
    [StandardUId],
    [Version],
    [EffectiveFrom],
    [EffectiveTo],
    [DateVersionApproved],
    [Comments],
    [Status]
    )
         VALUES (
         [Source].[OrganisationStandardId], 
         [Source].[StandardUId],
         [Source].[Version],
         [Source].[EffectiveFrom],
         [Source].[EffectiveTo],
         [Source].[DateVersionApproved],
         [Source].[Comments],
         [Source].[Status]
         );


-- Revert OrganisationStandardDeliveryArea to the "Before" data

DROP TABLE IF EXISTS #MergeOrganisationStandardDeliveryAreaBefore
SELECT * INTO #MergeOrganisationStandardDeliveryAreaBefore FROM MergeOrganisationStandardDeliveryArea WHERE MergeOrganisationId = @mergeOrganisationId AND Replicates = 'Before'

SET IDENTITY_INSERT [OrganisationStandardDeliveryArea] ON

MERGE [OrganisationStandardDeliveryArea] [Target] USING #MergeOrganisationStandardDeliveryAreaBefore [Source]
ON ([Source].OrganisationStandardDeliveryAreaId = [Target].Id)
WHEN MATCHED
    THEN UPDATE SET 
        [Target].[OrganisationStandardId] = [Source].[OrganisationStandardId],
        [Target].[DeliveryAreaId] = [Source].[DeliveryAreaId],
        [Target].[Comments] = [Source].[Comments],
        [Target].[Status] = [Source].[Status]

WHEN NOT MATCHED BY TARGET 
    THEN INSERT (
    [Id],
    [OrganisationStandardId], 
    [DeliveryAreaId],
    [Comments],
    [Status]    
    )
         VALUES (
         [Source].[Id], 
         [Source].[OrganisationStandardId], 
         [Source].[DeliveryAreaId],
         [Source].[Comments],
         [Source].[Status]
         );

SET IDENTITY_INSERT [OrganisationStandardDeliveryArea] OFF


-- Mark the merge as being undone

UPDATE MergeOrganisations SET Status = 'Reverted', UpdatedAt = SYSDATETIME(), UpdatedBy = @UpdatedBy WHERE Id = @mergeOrganisationId

-- Done

COMMIT TRANSACTION


RETURN 0