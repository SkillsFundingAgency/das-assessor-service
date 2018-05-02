﻿namespace SFA.DAS.AssessorService.Settings
{
    public interface ISftp
    {
        string Password { get; set; }
        int Port { get; set; }
        string RemoteHost { get; set; }
        string Username { get; set; }
        string UploadDirectory { get; set; }
        string ProofDirectory { get; set; }
    }
}