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
                        html += "<span class=\"block\">Reason: " + organisation.OrganisationData.RemovedReason.Reason + "</span>";
                    }
                    html += "<span class=\"block\">Updated: "+ organisation.StatusDate.ToString("dd MMM yyyy") + "</span>";
                    return new HtmlString(html);
                }
                case 2:
                {
                    var html = "<span class=\"govuk-tag govuk-!-margin-bottom-1\">Active</span>" +
                               "<span class=\"block\">Not accepting new apprentices</span>" +
                               "<span class=\"block\">Updated: " + organisation.StatusDate.ToString("dd MMM yyyy") + "</span>";
                    return new HtmlString(html);
                }
                default:
                {
                    var html = "<span class=\"govuk-tag govuk-!-margin-bottom-1\">Active</span>" +
                        "<span class=\"block\">Updated: " + organisation.StatusDate.ToString("dd MMM yyyy") + "</span>";
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
