-- migration part 2 OrganisationStandard

MERGE INTO OrganisationStandard MasterOS
USING (
SELECT os1.Id, [IfateReferenceNumber] from OrganisationStandard os1
LEFT JOIN (select [IfateReferenceNumber] , [Larscode] FROM standards WHERE LarsCode != 0 GROUP BY [IfateReferenceNumber] , [Larscode]
UNION SELECT 'ST0107',52 
UNION SELECT 'ST0457',115
) st1 ON St1.larscode = os1.StandardCode
) upd
ON MasterOS.Id = upd.Id
WHEN matched AND MasterOS.StandardReference IS NULL
THEN UPDATE
SET MasterOS.StandardReference = upd.IfateReferenceNumber;

