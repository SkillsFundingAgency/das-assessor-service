using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Extensions.TagHelpers
{
    [HtmlTargetElement(TagName)]
    public class SfaSortableColumnHeaderTagHelper : TagHelper
    {
        public const string TagName = "sfa-sortable-column-header";

        public const string AspActionName = "asp-action";
        public const string AspControllerName = "asp-controller";
        public const string AspHostName = "asp-host";
        public const string AspProtocolName = "asp-protocol";
        public const string AspFragmentName = "asp-fragment";

        public const string SfaSortColumnName = "sfa-sort-column";
        public const string SfaSortDirectionName = "sfa-sort-direction";
        public const string SfaTableSortColumnName = "sfa-table-sort-column";

        public const string DataSortDirectionName = "data-SortDirection";

        private readonly IHtmlGenerator _generator;

        public SfaSortableColumnHeaderTagHelper(IHtmlGenerator generator)
        {
            _generator = generator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(AspActionName)]
        public string AspAction { get; set; }

        [HtmlAttributeName(AspControllerName)]
        public string AspController { get; set; }

        [HtmlAttributeName(AspHostName)]
        public string AspHost { get; set; }

        [HtmlAttributeName(AspProtocolName)]
        public string AspProtocol { get; set; }

        [HtmlAttributeName(AspFragmentName)]
        public string AspFragment { get; set; }

        [HtmlAttributeName(SfaSortColumnName)]
        public string SfaSortColumn { get; set; }

        [HtmlAttributeName(SfaSortDirectionName)]
        public string SfaSortDirection { get; set; }

        [HtmlAttributeName(SfaTableSortColumnName)]
        public string SfaTableSortColumn { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            using (var writer = new StringWriter())
            {
                if (SfaSortColumn.Equals(SfaTableSortColumn, StringComparison.InvariantCultureIgnoreCase))
                {
                    var sortDirectionClass = SfaSortDirection == "Asc"
                        ? "sorted-ascending"
                        : "sorted-descending";

                    var classes = output.Attributes.FirstOrDefault(a => a.Name == "class")?.Value.ToString();

                    output.Attributes.SetAttribute("class",
                        string.IsNullOrEmpty(classes)
                            ? $"{sortDirectionClass}"
                            : $"{sortDirectionClass} {classes}");
                }

                output.Attributes.Add(DataSortDirectionName, new HtmlString(ToogleSortDirection()));

                var content = (await output.GetChildContentAsync()).GetContent();
                var link = _generator.GenerateActionLink(ViewContext, content,
                    AspAction, AspController, AspProtocol, AspHost, AspFragment,
                    new
                    {
                        SortColumn = SfaSortColumn,
                        SortDirection = ToogleSortDirection()
                    },
                    GetPassThroughAttributes(output));

                link.WriteTo(writer, NullHtmlEncoder.Default);

                output.TagName = null;
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.Clear();
                output.PostContent.SetHtmlContent(writer.ToString());
            }
        }

        private string ToogleSortDirection()
        {
            return SfaSortColumn == SfaTableSortColumn
                ? SfaSortDirection == "Asc"
                    ? "Desc"
                    : "Asc"
                : SfaSortDirection;
        }

        private Dictionary<string, object> GetPassThroughAttributes(TagHelperOutput output)
        {
            var passThroughAttributes = new Dictionary<string, object>();
            foreach (var attribute in output.Attributes)
            {
                passThroughAttributes[attribute.Name] = attribute.Value;
            }
            return passThroughAttributes;
        }
    }
}
