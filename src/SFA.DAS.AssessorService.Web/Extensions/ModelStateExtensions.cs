using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Extensions
{
    public static class ModelStateExtensions
    {
        public static T GetAttemptedValueWhenInvalid<T>(this ModelStateDictionary modelState, string key, T defaultValue)
        {
            if (modelState.IsValid)
            {
                return defaultValue;
            }

            if (!modelState.TryGetValue(key, out ModelStateEntry entry) || entry == null)
            {
                return defaultValue;
            }

            if(entry.AttemptedValue ==  null)
            {
                return defaultValue;
            }

            try
            {
                return (T)Convert.ChangeType(entry.AttemptedValue, typeof(T));
            }
            catch (InvalidCastException)
            {
                return defaultValue;
            }
        }

        public static List<T> GetAttemptedValueListWhenInvalid<T>(this ModelStateDictionary modelState, string key, List<T> defaultValue, char separator)
        {
            if (modelState.IsValid)
            {
                return defaultValue;
            }

            if (!modelState.TryGetValue(key, out ModelStateEntry entry) || entry == null)
            {
                return defaultValue;
            }

            var splitValues = entry.AttemptedValue?.Split(separator);
            if(splitValues == null)
            {
                return defaultValue;
            }

            try
            {
                return splitValues.Select(val => (T)Convert.ChangeType(val, typeof(T))).ToList();
            }
            catch (InvalidCastException)
            {
                return defaultValue;
            }
        }
    }
}
