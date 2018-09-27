namespace SFA.DAS.AssessorService.Api.Types.Models.Validation
{
    public class ValidationErrorDetail
    {
        public ValidationErrorDetail(string fieldDetails, string errorMessage)
        {
            FieldDetails = fieldDetails;
            ErrorMessage = errorMessage;
        }

        public ValidationErrorDetail(string fieldDetails, string errorMessage, ValidationStatusCode statusCode)
        {
            FieldDetails = fieldDetails;
            ErrorMessage = errorMessage;
            ValidationStatusCode = statusCode;
        }

        public ValidationErrorDetail(string errorMessage, ValidationStatusCode statusCode)
        {
            ErrorMessage = errorMessage;
            ValidationStatusCode = statusCode;
        }

        public string FieldDetails { get; set; }
        public string ErrorMessage { get; set; }
        protected ValidationStatusCode? ValidationStatusCode { get; set; }


        public string StatusCode => ValidationStatusCode.ToString();



    }
}