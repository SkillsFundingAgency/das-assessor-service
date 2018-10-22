﻿namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface ISpecialCharacterCleanserService
    {
        string CleanseStringForSpecialCharacters(string inputString);
        string UnescapeAndRemoveNonAlphanumericCharacters(string inputString);
    }
}