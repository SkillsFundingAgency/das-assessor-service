namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface ICertificateNameCapitalisationService
    {
        string ProperCase(string namePart, bool familyNamePart = false);
    }
}
