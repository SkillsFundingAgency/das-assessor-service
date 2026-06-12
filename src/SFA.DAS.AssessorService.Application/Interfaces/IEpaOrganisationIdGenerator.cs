namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IEpaOrganisationIdGenerator
    {
        string GetNextOrganisationId();
        string GetNextContactUsername();
    }
}
