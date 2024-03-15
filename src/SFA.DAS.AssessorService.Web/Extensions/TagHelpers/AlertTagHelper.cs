using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using System.Text.Encodings.Web;

namespace SFA.DAS.AssessorService.Web.Extensions.TagHelpers
{
    [HtmlTargetElement(TagName)]
    public class SfaAlertTagTagHelper : TagHelper
    {
        public const string TagName = "sfa-alert";

        public const string AlertSuccessClassName = "sfa-alert-success-class";
        public const string AlertInfoClassName = "sfa-alert-info-class";
        public const string AlertWarningClassName = "sfa-alert-warning-class";
        public const string AlertErrorClassName = "sfa-alert-error-class";
        public const string AlertMessageClassName = "sfa-alert-message-class";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(AlertSuccessClassName)]
        public string AlertSuccessClass { get; set; } = "success-summary";

        [HtmlAttributeName(AlertInfoClassName)]
        public string AlertInfoClass { get; set; } = "info-summary";

        [HtmlAttributeName(AlertWarningClassName)]
        public string AlertWarningClass { get; set; } = "warning-summary";

        [HtmlAttributeName(AlertErrorClassName)]
        public string AlertErrorClass { get; set; } = "error-summary";

        [HtmlAttributeName(AlertMessageClassName)]
        public string AlertMessageClass { get; set; } = "govuk-body";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var alert = ViewContext.TempData.GetAlert();
            if (alert == null)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "div";
            output.AddClass(GetAlertClass(alert), HtmlEncoder.Default);
            output.Content.SetHtmlContent($"<p class=\"{AlertMessageClass}\">{alert.Message}</p>");
            output.TagMode = TagMode.StartTagAndEndTag;
        }

        public string GetAlertClass(Alert alert)
        {
            switch(alert.Type)
            {
                case AlertType.Success:
                    return AlertSuccessClass;
                case AlertType.Info:
                    return AlertInfoClass;
                case AlertType.Warning:
                    return AlertWarningClass;
                case AlertType.Error:
                    return AlertErrorClass;
            }

            return AlertSuccessClass;
        }
    }
}
