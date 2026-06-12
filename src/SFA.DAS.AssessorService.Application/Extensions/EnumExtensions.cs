using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Extensions
{
    public static class EnumExtensions
    {
        public static List<string> ToFlagsList(this Enum value)
        {
            if (value == null)
                return new List<string>();

            var enumValues = Enum.GetValues(value.GetType())
                                 .Cast<Enum>()
                                 .Where(value.HasFlag)
                                 .Select(flag => flag.ToString())
                                 .ToList();

            return enumValues;
        }
    }

}
