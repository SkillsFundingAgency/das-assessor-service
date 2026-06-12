CREATE PROCEDURE [dbo].[StaffReports_List_Approved_Standards]
AS
BEGIN
DECLARE	@return_value int,
		@TotalCount int

EXEC	@return_value = [dbo].[OppFinder_List_Approved_Standards]
		@PageSize = 5000,
		@PageIndex = 1,
		@SearchTerm = '',
		@SectorFilters = '',
		@LevelFilters = '',
		@SortColumn = 'ActiveApprentices',
		@SortAscending = 0,
		@TotalCount = @TotalCount OUTPUT,
		@ApplyExclusions = 0

RETURN @return_value
END
