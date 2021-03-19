-- Migrate Apply table - set the Standardreference

MERGE INTO Apply MasterApply
USING (
SELECT ap1.Id ApplyId, st1.StandardReference
FROM Apply ap1 -- 986
JOIN
(SELECT [IFateReferenceNumber] StandardReference, [LarsCode] StandardCode
FROM (
SELECT [IFateReferenceNumber]  ,[LarsCode], ROW_NUMBER() OVER (PARTITION BY [LarsCode] ORDER BY  [VersionEarliestStartDate] DESC) rownumber
  FROM [dbo].[Standards]
  WHERE larscode != 0  -- 731
  AND [VersionEarliestStartDate] is not null
  ) ab WHERE rownumber = 1) st1 ON st1.StandardCode = ap1.StandardCode
  ) upd
  ON MasterApply.Id = upd.ApplyId
  WHEN MATCHED AND MasterApply.StandardReference IS NULL THEN 
  UPDATE SET MasterApply.StandardReference = upd.StandardReference;
