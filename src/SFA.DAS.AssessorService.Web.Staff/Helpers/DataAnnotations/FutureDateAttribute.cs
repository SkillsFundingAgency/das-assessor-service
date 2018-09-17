using System;
using System.ComponentModel.DataAnnotations;
using SFA.DAS.AssessorService.Domain.Extensions;

namespace SFA.DAS.AssessorService.Web.Staff.Helpers.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]

    public sealed class FutureDateAttribute : ValidationAttribute
    {
        public int GracePeriodInMinutes { set; get; }

        public FutureDateAttribute() : base() { }

        public override bool IsValid(object value)
        {
            DateTime date;

            if (value is null)
            {
                return false;
            }
            else if (value is DateTime)
            {
                date = Convert.ToDateTime(value).UtcFromTimeZoneTime();
            }
            else if (value is string)
            {
                DateTime.TryParse(value.ToString(), out date);
            }
            else
            {
                return false;
            }

            return date.AddMinutes(GracePeriodInMinutes) > DateTime.UtcNow;
        }
    }
}
