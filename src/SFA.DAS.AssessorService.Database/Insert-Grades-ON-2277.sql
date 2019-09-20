-- ON-2277 Create Grades table

DECLARE @rowCount int
SELECT @rowCount = COUNT(*) FROM Grades

IF @rowCount = 0 
BEGIN

	INSERT INTO Grades (GradeId,Grade,Pass,Enabled)
	VALUES
	('Pass','Pass',1,1),
	('Credit','Credit',1,1),
	('Merit','Merit',1,1),
	('Distinction','Distinction',1,1),
	('PassWithExcellence','Pass with excellence',1,1),
	('NoGradeAwarded','No grade awarded',1,1),
	('Fail','Fail',0,1);

END
GO