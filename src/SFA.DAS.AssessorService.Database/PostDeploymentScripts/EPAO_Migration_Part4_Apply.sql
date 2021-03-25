-- Migrate Apply table - set the Standardreference

UPDATE Apply
SET StandardReference = JSON_VALUE(ApplyData,'$.Apply.StandardReference') 
WHERE StandardReference IS NULL AND StandardCode IS NOT NULL
