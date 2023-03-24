using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Validators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal class NoHtmlAttribute : ValidationAttribute
    {
        private readonly string[] invalidInput = { "<", ">", "&lt;", "&gt;" };

        public override bool IsValid(object value)
        {
            var inputValue = value as string;
            return !invalidInput.Any(x => inputValue.Contains(x));
        }
    }
}
