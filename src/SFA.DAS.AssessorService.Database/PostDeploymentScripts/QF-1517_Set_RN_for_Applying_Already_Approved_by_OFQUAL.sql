-- In progress Apply already on OFQUAL list, so these will go Live (if not already done)
UPDATE [dbo].[organisations] SET [RecognitionNumber] ='RN6112' , [Status] = 'Live' WHERE EndPointAssessorOrganisationId = 'EPA0715' AND [RecognitionNumber] IS NULL;
UPDATE [dbo].[organisations] SET [RecognitionNumber] ='RN6117' , [Status] = 'Live' WHERE EndPointAssessorOrganisationId = 'EPA0826' AND [RecognitionNumber] IS NULL;
