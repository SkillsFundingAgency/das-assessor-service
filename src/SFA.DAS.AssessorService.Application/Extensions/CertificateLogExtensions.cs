using EnumsNET;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Enums;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Extensions
{
    public static class CertificateLogExtensions
    {
        public static void CalculateDifferences(this List<CertificateLogSummary> logs)
        {
            for (int i = 0; i < logs.Count(); i++)
            {
                var thisLog = logs[i];
                if (i != logs.Count() - 1)
                {
                    var prevLog = logs[i + 1];

                    thisLog.DifferencesToPrevious = GetChanges(thisLog, prevLog);
                }
                else
                {
                    thisLog.DifferencesToPrevious = new List<CertificateLogSummary.Difference>();
                }
            }
        }

        private static List<CertificateLogSummary.Difference> GetChanges(CertificateLogSummary thisLog, CertificateLogSummary prevLog)
        {
            var changes = new List<CertificateLogSummary.Difference>();

            var thisData = JsonConvert.DeserializeObject<CertificateData>(thisLog.CertificateData);
            var prevData = JsonConvert.DeserializeObject<CertificateData>(prevLog.CertificateData);

            // do not use generic change calculation for some properties
            var ignoreProperties = new string[] { nameof(CertificateData.EpaDetails),
                nameof(CertificateData.ReprintReasons), nameof(CertificateData.AmendReasons) };

            foreach (var propertyInfo in thisData.GetType().GetProperties())
            {
                if (ignoreProperties.Contains(propertyInfo.Name))
                    continue;

                var propertyIsList = propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType == typeof(List<string>);

                var thisProperty = propertyInfo.GetValue(thisData)?.ToString();
                var prevProperty = propertyInfo.GetValue(prevData)?.ToString();

                if (prevProperty is null && thisProperty is null)
                    continue;

                if (thisProperty != prevProperty)
                {
                    if (propertyInfo.PropertyType == typeof(DateTime) && DateTime.TryParse(thisProperty, out var result))
                    {
                        thisProperty = result.UtcToTimeZoneTime().ToShortDateString();
                    }

                    changes.Add(new CertificateLogSummary.Difference
                    {
                        Key = propertyInfo.Name.Spaceyfy(),
                        Values = new List<string>
                        {
                            string.IsNullOrEmpty(thisProperty)
                                ? "<Empty>"
                                : thisProperty
                        }
                    });
                }
            }

            // always populate the incident number and reprint or amend reasons in the changes
            if (thisLog.Action == CertificateActions.ReprintReason || thisLog.Action == CertificateActions.AmendReason)
            {
                if (!changes.Exists(p => p.Key == nameof(CertificateData.IncidentNumber).Spaceyfy()))
                {
                    changes.Add(new CertificateLogSummary.Difference { Key = nameof(CertificateData.IncidentNumber).Spaceyfy(), Values = new List<string> { thisData.IncidentNumber } });
                }

                if (thisLog.Action == CertificateActions.ReprintReason)
                {
                    var reprintReasons = thisData.ReprintReasons?.Select(p => Enum.TryParse(p, out ReprintReasons reprintReason)
                        ? reprintReason.AsString(EnumFormat.Description) : p).ToList();

                    changes.Add(new CertificateLogSummary.Difference { Key = nameof(CertificateData.ReprintReasons).Spaceyfy(), Values = reprintReasons, IsList = true });
                }
                else if (thisLog.Action == CertificateActions.AmendReason)
                {
                    var amendReasons = thisData.AmendReasons?.Select(p => Enum.TryParse(p, out AmendReasons amendReason)
                        ? amendReason.AsString(EnumFormat.Description) : p).ToList();

                    changes.Add(new CertificateLogSummary.Difference { Key = nameof(CertificateData.AmendReasons).Spaceyfy(), Values = amendReasons, IsList = true });
                }
            }

            return changes;
        }
    }

    public static class StringExtensions
    {
        public static string Spaceyfy(this string target)
        {
            var spaceyfiedString = target[0].ToString();
            foreach (var character in target.Skip(1))
            {
                if (char.IsUpper(character) || char.IsNumber(character))
                {
                    spaceyfiedString += " ";
                }

                spaceyfiedString += character;
            }

            return spaceyfiedString;
        }
    }
}
