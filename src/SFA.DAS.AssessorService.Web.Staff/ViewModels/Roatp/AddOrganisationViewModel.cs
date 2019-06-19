using System.Globalization;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
    using SFA.DAS.AssessorService.Api.Types.Models.Validation;

    public class AddOrganisationViewModel
    {
        public Guid OrganisationId { get; set; }
        public int ProviderTypeId { get; set; }
        public string LegalName { get; set; }
        public string UKPRN { get; set; }
        public int OrganisationTypeId { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public string TradingName { get; set; }
        public virtual DateTime? ApplicationDeterminedDate { get; set; }
        public IEnumerable<ProviderType> ProviderTypes { get; set; }
        public IEnumerable<OrganisationType> OrganisationTypes { get; set; }

    }

    public class AddOrganisationProviderTypeViewModel : AddOrganisationViewModel
    {
        public AddOrganisationProviderTypeViewModel()
        {
            OrganisationId = Guid.NewGuid();
        }

    }

    public class AddOrganisationTypeViewModel : AddOrganisationViewModel
    {
        public AddOrganisationTypeViewModel()
        {
            OrganisationId = Guid.NewGuid();
        }
    }

    public class AddApplicationDeterminedDateViewModel : AddOrganisationViewModel
    {
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public List<ValidationErrorDetail> ErrorMessages { get; set; }

        public bool IsError => ErrorMessages != null && ErrorMessages.Count > 0;

        public bool IsErrorDay => IsError && (ErrorMessages.Any(x => x.Field == "Day"));
        public bool IsErrorMonth => IsError && (ErrorMessages.Any(x => x.Field == "Month"));
        public bool IsErrorYear => IsError && (ErrorMessages.Any(x => x.Field == "Year"));

        public override DateTime? ApplicationDeterminedDate
        {
            get
            {
                var yearWithCentury = Year;
                if (yearWithCentury <= 99)
                    yearWithCentury +=2000;

                var formatStrings = new string[] { "d/M/yyyy" };
                if (!DateTime.TryParseExact($"{Day}/{Month}/{yearWithCentury}", formatStrings, null, DateTimeStyles.None,
                    out DateTime formattedDate))
                {
                    return null;
                }
                    return formattedDate;
            }
        }
    }

    public class AddOrganisationViaUkprnViewModel : AddOrganisationViewModel
    {
        public AddOrganisationViaUkprnViewModel()
        {
            OrganisationId = Guid.NewGuid();
        }
    }
}
