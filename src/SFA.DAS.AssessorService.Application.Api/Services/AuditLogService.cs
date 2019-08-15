using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Auditing;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class AuditLogService : IAuditLogService
    {
        public List<AuditLogDiff> Compare<T>(T previous, T current) where T : IAuditFilter
        {
            var auditDifferences = new List<AuditLogDiff>();

            CompareObjects(string.Empty, JObject.FromObject(previous), JObject.FromObject(current), auditDifferences);

            return auditDifferences?
                .Select(p => { p.FieldChanged = previous.FilterAuditDiff(p.ProperyChanged); return p; })
                .Where(p => !string.IsNullOrEmpty(p.FieldChanged)).ToList();
        }

        /// <summary>
        /// Deep compare two NewtonSoft JObjects. If they don't match, tracks differences
        /// </summary>
        /// <param name="parent">The property which contains the objects to be compared.</param>
        /// <param name="source">The expected results</param>
        /// <param name="target">The actual results</param>
        /// <param name="auditDifferences">The list to which differences will be appended</param>
        private void CompareObjects(string parent, JObject source, JObject target, List<AuditLogDiff> auditDifferences)
        {
            foreach (KeyValuePair<string, JToken> sourcePair in source)
            {
                if (sourcePair.Value.Type == JTokenType.Object)
                {
                    if (target.GetValue(sourcePair.Key) == null)
                    {
                        auditDifferences.Add(new AuditLogDiff
                        {
                            ProperyChanged = FormatPropertyChanged(parent, sourcePair.Key),
                            PreviousValue = sourcePair.Value.ToString(),
                            CurrentValue = string.Empty
                        });
                    }
                    else if (target.GetValue(sourcePair.Key).Type != JTokenType.Object)
                    {
                        auditDifferences.Add(new AuditLogDiff
                        {
                            ProperyChanged = FormatPropertyChanged(parent, sourcePair.Key),
                            PreviousValue = sourcePair.Value.ToString(),
                            CurrentValue = target.Property(sourcePair.Key).Value.ToString()
                        });
                    }
                    else
                    {
                        CompareObjects(parent + sourcePair.Key, sourcePair.Value.ToObject<JObject>(),
                            target.GetValue(sourcePair.Key).ToObject<JObject>(), auditDifferences);
                    }
                }
                else if (sourcePair.Value.Type == JTokenType.Array)
                {
                    if (target.GetValue(sourcePair.Key) == null)
                    {
                        auditDifferences.Add(new AuditLogDiff
                        {
                            ProperyChanged = FormatPropertyChanged(parent, sourcePair.Key),
                            PreviousValue = sourcePair.Value.ToString(),
                            CurrentValue = string.Empty
                        });
                    }
                    else
                    {
                        CompareArrays(parent + sourcePair.Key, sourcePair.Value.ToObject<JArray>(),
                            target.GetValue(sourcePair.Key).ToObject<JArray>(), auditDifferences, sourcePair.Key);
                    }
                }
                else
                {
                    JToken expected = sourcePair.Value;
                    var actual = target.SelectToken(sourcePair.Key);
                    if (actual == null)
                    {
                        auditDifferences.Add(new AuditLogDiff
                        {
                            ProperyChanged = FormatPropertyChanged(parent, sourcePair.Key),
                            PreviousValue = sourcePair.Value.ToString(),
                            CurrentValue = string.Empty
                        });
                    }
                    else
                    {
                        if (!JToken.DeepEquals(expected, actual))
                        {
                            auditDifferences.Add(new AuditLogDiff
                            {
                                ProperyChanged = FormatPropertyChanged(parent, sourcePair.Key),
                                PreviousValue = sourcePair.Value.ToString(),
                                CurrentValue = target.Property(sourcePair.Key).Value.ToString()
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deep compare two NewtonSoft JArrays. If they don't match, tracks differences
        /// </summary>
        /// <param name="parent">The property which contains the arrays to be compared.</param>
        /// <param name="source">The expected results</param>
        /// <param name="target">The actual results</param>
        /// <param name="auditDifferences">The list to which differences will be appended</param>
        /// <param name="arrayName">The name of the array to use in the audit diff property name</param>
        private void CompareArrays(string parent, JArray source, JArray target, List<AuditLogDiff> auditDifferences, string arrayName = "")
        {
            for (var index = 0; index < (source?.Count ?? 0); index++)
            {
                var expected = source[index];
                if (expected.Type == JTokenType.Object)
                {
                    var actual = (index >= (target?.Count ?? 0)) ? new JObject() : target[index];
                    CompareObjects(FormatPropertyChanged(parent, arrayName), expected.ToObject<JObject>(),
                        actual.ToObject<JObject>(), auditDifferences);
                }
                else
                {
                    var actual = (index >= (target?.Count ?? 0)) ? "" : target[index];
                    if (!JToken.DeepEquals(expected, actual))
                    {
                        auditDifferences.Add(new AuditLogDiff
                        {
                            ProperyChanged = FormatPropertyChanged(parent, $"{arrayName}[{index}]"),
                            PreviousValue = expected.ToString(),
                            CurrentValue = actual.ToString()
                        });
                    }
                }
            }
        }

        private string FormatPropertyChanged(string parent, string child)
        {
            return $"{(string.IsNullOrEmpty(parent) ? string.Empty : parent + ".")}{child}";
        }
    }
}
