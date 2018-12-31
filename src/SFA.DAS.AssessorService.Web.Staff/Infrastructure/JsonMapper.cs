using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public static class JsonMapper
    {
        /// <summary>
        /// Replaces sourceJson attributes with what is in the mappingJson
        /// </summary>
        /// <param name="mappingJson"></param>
        /// <param name="sourceJson"></param>
        /// <param name="destinationDictionary"></param>
        public static void MatchAndCreateNewMapping(string mappingJson, dynamic sourceJson, Dictionary<string, object> destinationDictionary)
        {
            var mappings = JsonConvert.DeserializeObject<Dictionary<string, object>>(mappingJson);
            foreach (var mapping in mappings)
            {
                foreach (KeyValuePair<string, object> source in sourceJson)
                {
                    if (mapping.Key != source.Key) continue;
                    if (IsValidJson(mapping.Value.ToString()))
                    {
                        var child = JObject.Parse(mapping.Value.ToString());
                        foreach (var property in child)
                        {
                            dynamic childMapping = property.Value;

                            dynamic childSource = source.Value;
                            var childList = new List<Dictionary<string, object>>();
                            if (childSource.Count != null)
                            {
                                for (var i = 0; i < childSource.Count; i++)
                                    ProcessNestedJson(childSource[i], childMapping, childList);
                            }
                            else
                                ProcessNestedJson(childSource, childMapping, childList);

                            if (childList.Any())
                                destinationDictionary.Add(property.Key, childList);
                            break;
                        }
                    }
                    else
                    {
                        destinationDictionary.Add(mapping.Value.ToString(), source.Value);
                    }
                    break;
                }
            }
        }


        /// <summary>
        /// Processes child Json Objects iteratively calling back into MatchAndCreateNewMapping
        /// </summary>
        /// <param name="childSource"></param>
        /// <param name="childMapping"></param>
        /// <param name="childList"></param>
        private static void ProcessNestedJson(dynamic childSource, dynamic childMapping, List<Dictionary<string, object>> childList)
        {
            var childDestinationDictionary = new Dictionary<string, object>();
            dynamic keyVal = JsonConvert.DeserializeObject<Dictionary<string, object>>(childSource.ToString());

            MatchAndCreateNewMapping(childMapping.ToString(),
                keyVal,
                childDestinationDictionary);
            childList.Add(JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(childDestinationDictionary)));
        }

        /// <summary>
        /// Checks if string has a valid JSON format
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
                catch (Exception) 
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
