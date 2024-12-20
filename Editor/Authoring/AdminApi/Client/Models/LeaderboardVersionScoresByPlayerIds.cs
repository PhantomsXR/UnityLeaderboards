//-----------------------------------------------------------------------------
// <auto-generated>
//     This file was generated by the C# SDK Code Generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Unity.Services.Leaderboards.Authoring.Client.Http;



namespace Unity.Services.Leaderboards.Authoring.Client.Models
{
    /// <summary>
    /// LeaderboardVersionScoresByPlayerIds model
    /// </summary>
    [Preserve]
    [DataContract(Name = "LeaderboardVersionScoresByPlayerIds")]
    internal class LeaderboardVersionScoresByPlayerIds
    {
        /// <summary>
        /// Creates an instance of LeaderboardVersionScoresByPlayerIds.
        /// </summary>
        /// <param name="version">version param</param>
        /// <param name="results">results param</param>
        /// <param name="entriesNotFoundForPlayerIds">entriesNotFoundForPlayerIds param</param>
        [Preserve]
        public LeaderboardVersionScoresByPlayerIds(LeaderboardVersion2 version = default, List<object> results = default, List<string> entriesNotFoundForPlayerIds = default)
        {
            Version = version;
            Results = (List<IDeserializable>) JsonObject.GetNewJsonObjectResponse(results);
            EntriesNotFoundForPlayerIds = entriesNotFoundForPlayerIds;
        }

        /// <summary>
        /// Parameter version of LeaderboardVersionScoresByPlayerIds
        /// </summary>
        [Preserve]
        [DataMember(Name = "version", EmitDefaultValue = false)]
        public LeaderboardVersion2 Version{ get; }
        
        /// <summary>
        /// Parameter results of LeaderboardVersionScoresByPlayerIds
        /// </summary>
        [Preserve]
        [DataMember(Name = "results", EmitDefaultValue = false)]
        public List<IDeserializable> Results{ get; }
        
        /// <summary>
        /// Parameter entriesNotFoundForPlayerIds of LeaderboardVersionScoresByPlayerIds
        /// </summary>
        [Preserve]
        [DataMember(Name = "entriesNotFoundForPlayerIds", EmitDefaultValue = false)]
        public List<string> EntriesNotFoundForPlayerIds{ get; }
    
        /// <summary>
        /// Formats a LeaderboardVersionScoresByPlayerIds into a string of key-value pairs for use as a path parameter.
        /// </summary>
        /// <returns>Returns a string representation of the key-value pairs.</returns>
        internal string SerializeAsPathParam()
        {
            var serializedModel = "";

            if (Version != null)
            {
                serializedModel += "version," + Version.ToString() + ",";
            }
            if (Results != null)
            {
                serializedModel += "results," + Results.ToString() + ",";
            }
            if (EntriesNotFoundForPlayerIds != null)
            {
                serializedModel += "entriesNotFoundForPlayerIds," + EntriesNotFoundForPlayerIds.ToString();
            }
            return serializedModel;
        }

        /// <summary>
        /// Returns a LeaderboardVersionScoresByPlayerIds as a dictionary of key-value pairs for use as a query parameter.
        /// </summary>
        /// <returns>Returns a dictionary of string key-value pairs.</returns>
        internal Dictionary<string, string> GetAsQueryParam()
        {
            var dictionary = new Dictionary<string, string>();

            if (Results != null)
            {
                var resultsStringValue = Results.ToString();
                dictionary.Add("results", resultsStringValue);
            }
            
            if (EntriesNotFoundForPlayerIds != null)
            {
                var entriesNotFoundForPlayerIdsStringValue = EntriesNotFoundForPlayerIds.ToString();
                dictionary.Add("entriesNotFoundForPlayerIds", entriesNotFoundForPlayerIdsStringValue);
            }
            
            return dictionary;
        }
    }
}
