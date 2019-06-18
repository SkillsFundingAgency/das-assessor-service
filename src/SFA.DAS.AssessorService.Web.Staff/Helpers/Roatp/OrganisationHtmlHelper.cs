namespace SFA.DAS.AssessorService.Web.Staff.Helpers.Roatp
{
    using Api.Types.Models.Roatp;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class OrganisationHtmlHelper
    {
        public static IHtmlContent OrganisationStatus(this IHtmlHelper htmlHelper, Organisation organisation)
        {
            switch (organisation.OrganisationStatus.Id)
            {
                case 0:
                {
                    var html = "<span class=\"govuk-tag govuk-tag--attention govuk-!-margin-bottom-1\">Removed</span>";
                    if (organisation.OrganisationData.RemovedReason != null)
                    {
                        html += "<p class=\"govuk-body govuk-!-margin-bottom-1\">Reason: " + organisation.OrganisationData.RemovedReason.Reason + "</p>";
                    }
                    html += "<p class=\"govuk-body\">Updated: "+ organisation.StatusDate.ToString("dd MMM yyyy") + "</p>";
                    return new HtmlString(html);
                }
                case 2:
                {
                    var html = "<span class=\"govuk-tag govuk-!-margin-bottom-1\">Active</span>" +
                               "<p class=\"govuk-body govuk-!-margin-bottom-1\">Not accepting new apprentices</span>" +
                               "<p class=\"govuk-body\">Updated: " + organisation.StatusDate.ToString("dd MMM yyyy") + "</p>";
                    return new HtmlString(html);
                }
                case 3:
                {
                    var html = "<span class=\"govuk-tag govuk-tag--onboarding govuk-!-margin-bottom-1\">On-boarding</span>" +
                               "<p class=\"govuk-body\">Updated: " + organisation.StatusDate.ToString("dd MMM yyyy") + "</p>";
                    return new HtmlString(html);
                }
                default:
                {
                    var html = "<span class=\"govuk-tag govuk-!-margin-bottom-1\">Active</span>" +
                        "<p class=\"govuk-body\">Updated: " + organisation.StatusDate.ToString("dd MMM yyyy") + "</p>";
                    return new HtmlString(html);                   
                }
            }
        }

        public static IHtmlContent ProviderType(this IHtmlHelper htmlHelper, Organisation organisation)
        {
            switch (organisation.ProviderType.Id)
            {
                case 2:
                {
                    return new HtmlString("Employer");
                }
                case 3:
                {
                    return new HtmlString("Supporting");
                }
                default:
                {
                    return new HtmlString("Main");
                }
            }
        }

        public static IHtmlContent BooleanYesNo(this IHtmlHelper htmlHelper, bool booleanValue)
        {
            string formattedValue = "No";
            if (booleanValue)
            {
                formattedValue = "Yes";
            }

            return new HtmlString(formattedValue);
        }
    }
}
