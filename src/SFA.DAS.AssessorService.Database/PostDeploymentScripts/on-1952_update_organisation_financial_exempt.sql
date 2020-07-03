BEGIN TRANSACTION
	UPDATE OrganisationType SET FinancialExempt = 1 WHERE Type = 'HEI' AND FinancialExempt = 0
	UPDATE OrganisationType SET FinancialExempt = 1 WHERE Type = 'Public Sector' AND FinancialExempt = 0
	UPDATE OrganisationType SET FinancialExempt = 1 WHERE Type = 'College' AND FinancialExempt = 0 
	UPDATE OrganisationType SET FinancialExempt = 1 WHERE Type = 'Academy or Free School' AND FinancialExempt = 0
COMMIT TRANSACTION