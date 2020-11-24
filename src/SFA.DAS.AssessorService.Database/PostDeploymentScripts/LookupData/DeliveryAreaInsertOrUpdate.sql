/*
	Insert or Update each of the [DeliveryArea] look up default values.

	NOTES:

	1) This script uses a temporary table, insert or update the values in the temporary table to apply changes; removed values will
	not take affect (by design); values which are removed should also be written into the DeliveryAreaDelete.sql script to remove
	manually any dependencies, but they must also be removed from the temporary table.
*/
BEGIN TRANSACTION

CREATE TABLE #DeliveryArea(
	[Id] [int] NOT NULL,
	[Area] [nvarchar](256) NOT NULL,
	[Status] [nvarchar](10) NOT NULL,
	[Ordering] [int] NOT NULL,
) 

INSERT #DeliveryArea VALUES (1, N'East Midlands', N'Live', 4)
INSERT #DeliveryArea VALUES (2, N'East of England', N'Live', 6)
INSERT #DeliveryArea VALUES (3, N'London', N'Live', 7)
INSERT #DeliveryArea VALUES (4, N'North East', N'Live', 1)
INSERT #DeliveryArea VALUES (5, N'North West', N'Live', 2)
INSERT #DeliveryArea VALUES (6, N'South East', N'Live', 8)
INSERT #DeliveryArea VALUES (7, N'South West', N'Live', 9)
INSERT #DeliveryArea VALUES (8, N'West Midlands', N'Live', 5)
INSERT #DeliveryArea VALUES (9, N'Yorkshire and the Humber', N'Live', 3)

SET IDENTITY_INSERT [dbo].[DeliveryArea] ON 

MERGE [DeliveryArea] [Target] USING #DeliveryArea [Source]
ON ([Source].Id = [Target].Id)
WHEN MATCHED
    THEN UPDATE SET 
        [Target].[Area] = [Source].[Area],
        [Target].[Status] = [Source].[Status],
        [Target].[Ordering] = [Source].[Ordering]

WHEN NOT MATCHED BY TARGET 
    THEN INSERT ([Id], [Area], [Status], [Ordering])
         VALUES ([Source].[Id], [Source].[Area], [Source].[Status], [Source].[Ordering]);

SET IDENTITY_INSERT [dbo].[DeliveryArea] OFF

COMMIT TRANSACTION
