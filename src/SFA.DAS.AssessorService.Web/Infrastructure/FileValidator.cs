using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public class FileValidator
    {
        public static bool FileValidationPassed(List<Answer> answers, Page page, List<ValidationErrorDetail> errorMessages, ModelStateDictionary modelState, IFormFileCollection files)
        {
            var fileValidationPassed = true;
            if (!files.Any()) return true;

            foreach (var file in files)
            {

                var typeValidation = page.Questions.FirstOrDefault(q => q.QuestionId == file.Name)?.Input.Validations.FirstOrDefault(v => v.Name == "FileType");
                if (typeValidation != null)
                {
                    var allowedExtension = typeValidation.Value.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries)[0];
                    var mimeType = typeValidation.Value.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries)[1];

                    var fileNameParts = file.FileName.Split(".", StringSplitOptions.RemoveEmptyEntries);
                    var fileNameExtension = fileNameParts[fileNameParts.Length - 1];

                    if (fileNameExtension != allowedExtension || file.ContentType.ToLower() != mimeType)
                    {
                        modelState.AddModelError(file.Name, typeValidation.ErrorMessage);
                        errorMessages.Add(new ValidationErrorDetail(file.Name, typeValidation.ErrorMessage));
                        fileValidationPassed = false;
                    }
                    else
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
                            answers.Add(new Answer() { QuestionId = file.Name, Value = new []{ file.FileName }});
                        }
                    }
                }
                else
                {
                    // Only add to answers if type validation passes.
                    answers.Add(new Answer() { QuestionId = file.Name, Value = new []{ file.FileName }});
                }
            }

            return fileValidationPassed;
        }

    }
}
