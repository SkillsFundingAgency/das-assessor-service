namespace SFA.DAS.AssessorService.Application.Logging
{
    public class LoggingConstants
    {
        public const string SignInSuccessful = "SignIn_Successful";
        public const string SignInIncorrectRole = "SignIn_IncorrectRole";
        public const string SignInNotAnEpao = "SignIn_NotAnEpao";
        public const string SignInEpaoDeleted = "SignIn_EpaoDeleted";
        public const string SignInEpaoNew = "SignIn_EpaoNew";

        public const string SearchSuccess = "Search_Success";
        public const string SearchFailure = "Search_Failure";
        
        public const string CertificateStarted = "Certificate_Started";
        public const string CertificateSubmitted = "Certificate_Submitted";
        public const string CertificatePrinted = "Certificate_Printed";
    }
}