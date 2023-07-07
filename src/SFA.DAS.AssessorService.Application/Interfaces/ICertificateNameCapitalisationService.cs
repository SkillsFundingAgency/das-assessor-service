namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface ICertificateNameCapitalisationService
    {
        string ProperCase(string namePart, bool familyNamePart = false);
        void CleanseStringForSpecialCharacters(ref string inputString);
        char[] SpecialCharactersInString(string inputString);
        void FixConjunction(ref string source);
        void UpdateMac(ref string source);

    }
}
