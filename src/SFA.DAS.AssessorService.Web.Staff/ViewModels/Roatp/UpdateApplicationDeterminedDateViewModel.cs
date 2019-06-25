using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{
    public class UpdateApplicationDeterminedDateViewModel
    {
        public int? Day { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }

        public  DateTime? ApplicationDeterminedDate
        {
            get
            {
                var yearWithCentury = Year;
                if (yearWithCentury <= 99)
                    yearWithCentury += 2000;

                var formatStrings = new string[] { "d/M/yyyy" };
                if (!DateTime.TryParseExact($"{Day}/{Month}/{yearWithCentury}", formatStrings, null, DateTimeStyles.None,
                    out DateTime formattedDate))
                {
                    return null;
                }
                return formattedDate;
            }
        }

        public List<ValidationErrorDetail> ErrorMessages { get; set; }

        public bool IsError => ErrorMessages != null && ErrorMessages.Count > 0;

        public bool IsErrorDay => IsError && (ErrorMessages.Any(x => x.Field == "Day"));
        public bool IsErrorMonth => IsError && (ErrorMessages.Any(x => x.Field == "Month"));
        public bool IsErrorYear => IsError && (ErrorMessages.Any(x => x.Field == "Year"));

        public Guid OrganisationId { get; set; }
        public string LegalName { get; set; }

        public string UpdatedBy { get; set; }
    }
}
