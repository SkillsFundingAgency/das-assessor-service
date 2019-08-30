using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using SFA.DAS.AssessorService.Api.Types.Attributes;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Auditing;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly IRegisterQueryRepository _registerQueryRepository;

        public AuditLogService(IAuditRepository auditRepository, IRegisterQueryRepository registerQueryRepository)
        {
            _auditRepository = auditRepository;
            _registerQueryRepository = registerQueryRepository;
        }

        public async Task<List<AuditChange>> GetEpaOrganisationChanges(string organisationId, EpaOrganisation organisation)
        {
            // get the current organisation to compare, removing the primary contact information which is not updated by this request
            var prevOrganisation = await _registerQueryRepository.GetEpaOrganisationByOrganisationId(organisationId);
            prevOrganisation.PrimaryContact = null;

            return Compare<EpaOrganisation>(prevOrganisation, organisation);
        }

        public async Task<List<AuditChange>> GetEpaOrganisationPrimaryContactChanges(string organisationId, EpaContact primaryContact)
        {
            var attributes = typeof(EpaOrganisation).GetCustomAttributes(typeof(AuditFilterAttribute), true);
            if (attributes.Length == 0)
                throw new ArgumentException("T does not have attribute AuditFilterAttribute");

            if (attributes[0] is AuditFilterAttribute auditFilterAttribute)
            {
                var auditChanges = new List<AuditChange>();

                // get the current organisation details and compare to the new primary contact username
                var prevOrganisation = await _registerQueryRepository.GetEpaOrganisationByOrganisationId(organisationId);
                if(!string.Equals(prevOrganisation.PrimaryContact, primaryContact.Username, StringComparison.InvariantCultureIgnoreCase))
                {
                    auditChanges.Add(new AuditChange
                    {
                        ProperyChanged = $"{nameof(EpaOrganisation.PrimaryContact)}",
                        PreviousValue = prevOrganisation.PrimaryContact,
                        CurrentValue = primaryContact.Username
                    });
                }

                var auditFilter = Activator.CreateInstance(auditFilterAttribute.AuditFilter) as IAuditFilter;
                return auditChanges?
                    .Select(p => { p.FieldChanged = auditFilter.FilterAuditDiff(p.ProperyChanged); return p; })
                    .Where(p => !string.IsNullOrEmpty(p.FieldChanged)).ToList();
            }

            return null;
        }

        public async Task WriteChangesToAuditLog(string organisationId, string updatedBy, List<AuditChange> auditChanges)
        {
            var organisation = await _registerQueryRepository.GetEpaOrganisationByOrganisationId(organisationId);
            await _auditRepository.CreateAudit(organisation.Id, updatedBy, auditChanges);
        }

        public List<AuditChange> Compare<T>(T previous, T current)
        {
            var attributes = typeof(T).GetCustomAttributes(typeof(AuditFilterAttribute), true);
            if (attributes.Length == 0)
                throw new ArgumentException("T does not have attribute AuditFilterAttribute");

            if (attributes[0] is AuditFilterAttribute auditFilterAttribute)
            {
                var auditChanges = new List<AuditChange>();
                
                CompareObjects(string.Empty, JObject.FromObject(previous), JObject.FromObject(current), auditChanges);

                var auditFilter = Activator.CreateInstance(auditFilterAttribute.AuditFilter) as IAuditFilter;
                return auditChanges?
                    .Select(p => { p.FieldChanged = auditFilter.FilterAuditDiff(p.ProperyChanged); return p; })
                    .Where(p => !string.IsNullOrEmpty(p.FieldChanged)).ToList();
            }

            return null;
        }

        /// <summary>
        /// Deep compare two NewtonSoft JObjects. If they don't match, tracks differences
        /// </summary>
        /// <param name="parent">The property which contains the objects to be compared.</param>
        /// <param name="source">The expected results</param>
        /// <param name="target">The actual results</param>
        /// <param name="auditChanges">The list to which differences will be appended</param>
        private void CompareObjects(string parent, JObject source, JObject target, List<AuditChange> auditChanges)
        {
            foreach (KeyValuePair<string, JToken> sourcePair in source)
            {
                if (sourcePair.Value.Type == JTokenType.Object)
                {
                    if (target.GetValue(sourcePair.Key) == null)
                    {
                        auditChanges.Add(new AuditChange
                        {
                            ProperyChanged = FormatPropertyChanged(parent, sourcePair.Key),
                            PreviousValue = sourcePair.Value.ToString(),
                            CurrentValue = string.Empty
                        });
                    }
                    else if (target.GetValue(sourcePair.Key).Type != JTokenType.Object)
                    {
                        auditChanges.Add(new AuditChange
                        {
                            ProperyChanged = FormatPropertyChanged(parent, sourcePair.Key),
                            PreviousValue = sourcePair.Value.ToString(),
                            CurrentValue = target.Property(sourcePair.Key).Value.ToString()
                        });
                    }
                    else
                    {
                        CompareObjects(parent + sourcePair.Key, sourcePair.Value.ToObject<JObject>(),
                            target.GetValue(sourcePair.Key).ToObject<JObject>(), auditChanges);
                    }
                }
                else if (sourcePair.Value.Type == JTokenType.Array)
                {
                    if (target.GetValue(sourcePair.Key) == null)
                    {
                        auditChanges.Add(new AuditChange
                        {
                            ProperyChanged = FormatPropertyChanged(parent, sourcePair.Key),
                            PreviousValue = sourcePair.Value.ToString(),
                            CurrentValue = string.Empty
                        });
                    }
                    else
                    {
                        CompareArrays(parent + sourcePair.Key, sourcePair.Value.ToObject<JArray>(),
                            target.GetValue(sourcePair.Key).ToObject<JArray>(), auditChanges, sourcePair.Key);
                    }
                }
                else
                {
                    JToken expected = sourcePair.Value;
                    var actual = target.SelectToken(sourcePair.Key);
                    if (actual == null)
                    {
                        auditChanges.Add(new AuditChange
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
                            auditChanges.Add(new AuditChange
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
        /// <param name="auditChanges">The list to which differences will be appended</param>
        /// <param name="arrayName">The name of the array to use in the audit diff property name</param>
        private void CompareArrays(string parent, JArray source, JArray target, List<AuditChange> auditChanges, string arrayName = "")
        {
            for (var index = 0; index < (source?.Count ?? 0); index++)
            {
                var expected = source[index];
                if (expected.Type == JTokenType.Object)
                {
                    var actual = (index >= (target?.Count ?? 0)) ? new JObject() : target[index];
                    CompareObjects(FormatPropertyChanged(parent, arrayName), expected.ToObject<JObject>(),
                        actual.ToObject<JObject>(), auditChanges);
                }
                else
                {
                    var actual = (index >= (target?.Count ?? 0)) ? "" : target[index];
                    if (!JToken.DeepEquals(expected, actual))
                    {
                        auditChanges.Add(new AuditChange
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
