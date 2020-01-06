using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public class FileValidator
    {
        public static bool FileValidationPassed(List<Answer> answers, Page page, List<ValidationErrorDetail> errorMessages, ModelStateDictionary modelState, IFormFileCollection files)
        {
            ValidationDefinition typeValidation = null;
            var fileValidationPassed = true;

            if (answers != null && answers[0]?.Value == string.Empty)
            {
                typeValidation = page.Questions.FirstOrDefault(q => q.QuestionId == answers[0].QuestionId)?.Input.Validations.FirstOrDefault(v => v.Name == "Required");
                if (typeValidation != null)
                {
                    modelState.AddModelError(answers[0].QuestionId, typeValidation.ErrorMessage);
                    errorMessages.Add(new ValidationErrorDetail(answers[0].QuestionId, typeValidation.ErrorMessage));
                    fileValidationPassed = false;
                }
            }
            foreach (var file in files)
            {

                typeValidation = page.Questions.FirstOrDefault(q => q.QuestionId == file.Name)?.Input.Validations.FirstOrDefault(v => v.Name == "FileType");
                if (typeValidation != null)
                {
                    if(file.Length > 0)
                    {
                        var size = (file.Length / 1024f) / 1024f;
                        if (size > 5d)
                        {
                            modelState.AddModelError(file.Name, "The PDF file must be smaller than 5MB.");
                            errorMessages.Add(new ValidationErrorDetail(file.Name, "The PDF file must be smaller than 5MB."));
                            fileValidationPassed = false;
                        }
                        else
                        {
                            // Only add to answers if type validation passes.
                            answers.Add(new Answer() { QuestionId = file.Name, Value = file.FileName });
                        }
                    }
                }
                else
                {
                    // Only add to answers if type validation passes.
                    answers.Add(new Answer() { QuestionId = file.Name, Value = file.FileName });
                }
            }

            return fileValidationPassed;
        }

    }
}
