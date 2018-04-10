namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Settings
{
    public interface ISftp
    {
        string Password { get; set; }
        int Port { get; set; }
        string RemoteHost { get; set; }
        string Username { get; set; }
    }
}