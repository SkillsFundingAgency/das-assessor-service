using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.QnA.Api.Types.Page;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public class FileValidator
    {
        public static bool FileValidationPassed(List<Answer> answers, Page page, List<ValidationErrorDetail> errorMessages, ModelStateDictionary modelState, IFormFileCollection files)
        {
            var fileValidationPassed = true;

            if (answers != null)
            {
                foreach (var fileAnswer in answers)
                {
                    // Check if it's a required answer
                    var typeValidation = page.Questions.FirstOrDefault(q => q.QuestionId == fileAnswer.QuestionId)?.Input.Validations.FirstOrDefault(v => v.Name == "Required");
                    if (typeValidation != null && string.IsNullOrWhiteSpace(fileAnswer.Value))
                    {
                        modelState.AddModelError(answers[0].QuestionId, typeValidation.ErrorMessage);
                        errorMessages.Add(new ValidationErrorDetail(answers[0].QuestionId, typeValidation.ErrorMessage));
                        fileValidationPassed = false;
                    }
                }
            }

            if (files != null)
            {
                foreach (var file in files)
                {
                    // Check if needs to be smaller than 5MB
                    var typeValidation = page.Questions.FirstOrDefault(q => q.QuestionId == file.Name)?.Input.Validations.FirstOrDefault(v => v.Name == "FileType");
                    if (typeValidation != null)
                    {
                        if (file.Length > 0)
                        {
                            var size = (file.Length / 1024f) / 1024f;
                            if (size > 5d)
                            {
                                fileValidationPassed = false;

                                modelState.AddModelError(file.Name, "The PDF file must be smaller than 5MB.");
                                errorMessages.Add(new ValidationErrorDetail(file.Name, "The PDF file must be smaller than 5MB."));
                            }
                        }
                    }
                }
            }

            return fileValidationPassed;
        }

        public static bool AllRequiredFilesArePresent(Page page, List<ValidationErrorDetail> errorMessages, ModelStateDictionary modelState)
        {
            var allRequiredFilesArePresent = true;

            var requiredFileUploads = page.Questions.Where(q => q.Input.Type == "FileUpload" && q.Input.Validations.Any(v => v.Name == "Required"));

            // Check an answer has been supplied for the required file upload
            foreach (var requiredUpload in requiredFileUploads)
            {
                // Search through answers for the question and check it's not null or empty
                var hasRequiredUpload = page.PageOfAnswers.SelectMany(pao => pao.Answers).Any(a => a.QuestionId == requiredUpload.QuestionId && !string.IsNullOrEmpty(a.Value));

                if (!hasRequiredUpload)
                {
                    var typeValidation = requiredUpload.Input.Validations.First(v => v.Name == "Required");
                    modelState.AddModelError(requiredUpload.QuestionId, typeValidation.ErrorMessage);
                    errorMessages.Add(new ValidationErrorDetail(requiredUpload.QuestionId, typeValidation.ErrorMessage));
                    allRequiredFilesArePresent = false;
                }
            }

            return allRequiredFilesArePresent;
        }

    }
}
