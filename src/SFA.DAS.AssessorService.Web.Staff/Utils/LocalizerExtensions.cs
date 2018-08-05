using System;
using Microsoft.AspNetCore.Mvc.Localization;

namespace SFA.DAS.AssessorService.Web.Staff.Utils
{
    public static class LocalizerExtensions
    {
        public static LocalizedHtmlString Pluralize(this IViewLocalizer localizer, string key, bool isPlural)
        {
            var resource = localizer[key];
            if (resource.IsResourceNotFound)
            {
                return resource;
            }
            
            if (!resource.Value.Contains("|"))
            {
                throw new ArgumentException("The resource values must contain a pipe (|) separated pair of values.");
            }

            var parts = resource.Value.Split('|', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                throw new ArgumentException("The resource values must contain a pipe (|) separated pair of values.");
            }

            return isPlural ? new LocalizedHtmlString(key, parts[1]) : new LocalizedHtmlString(key, parts[0]);
        }
    }
}