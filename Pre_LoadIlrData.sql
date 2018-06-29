-- Convert all ULNs to negative that will be removed should the load succeed.
UPDATE Ilrs SET Uln = 0 - Uln WHERE Source = 'Extract'