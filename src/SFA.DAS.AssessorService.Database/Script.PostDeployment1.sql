/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
IF NOT EXISTS(SELECT * FROM ScheduleRuns)
BEGIN
	INSERT INTO ScheduleRuns (RunTime, IsComplete, Interval, IsRecurring, ScheduleType) VALUES ('2018-08-17 08:00:00', 0, 10080, 1, 1)
END