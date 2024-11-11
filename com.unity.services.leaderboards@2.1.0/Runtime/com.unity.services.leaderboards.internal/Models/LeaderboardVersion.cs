using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Unity.Services.Leaderboards.Internal.Http;



namespace Unity.Services.Leaderboards.Internal.Models
{
    /// <summary>
    /// LeaderboardVersion model
    /// </summary>
    [Preserve]
    [DataContract(Name = "LeaderboardVersion")]
    internal class LeaderboardVersion
    {
        /// <summary>
        /// Creates an instance of LeaderboardVersion.
        /// </summary>
        /// <param name="id">id param</param>
        /// <param name="start">start param</param>
        /// <param name="end">end param</param>
        [Preserve]
        public LeaderboardVersion(string id = default, DateTime start = default, DateTime end = default)
        {
            Id = id;
            Start = start;
            End = end;
        }

        /// <summary>
        /// Parameter id of LeaderboardVersion
        /// </summary>
        [Preserve]
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id{ get; }

        /// <summary>
        /// Parameter start of LeaderboardVersion
        /// </summary>
        [Preserve]
        [DataMember(Name = "start", EmitDefaultValue = false)]
        public DateTime Start{ get; }

        /// <summary>
        /// Parameter end of LeaderboardVersion
        /// </summary>
        [Preserve]
        [DataMember(Name = "end", EmitDefaultValue = false)]
        public DateTime End{ get; }

        /// <summary>
        /// Formats a LeaderboardVersion into a string of key-value pairs for use as a path parameter.
        /// </summary>
        /// <returns>Returns a string representation of the key-value pairs.</returns>
        internal string SerializeAsPathParam()
        {
            var serializedModel = "";

            if (Id != null)
            {
                serializedModel += "id," + Id + ",";
            }
            if (Start != null)
            {
                serializedModel += "start," + Start.ToString() + ",";
            }
            if (End != null)
            {
                serializedModel += "end," + End.ToString();
            }
            return serializedModel;
        }

        /// <summary>
        /// Returns a LeaderboardVersion as a dictionary of key-value pairs for use as a query parameter.
        /// </summary>
        /// <returns>Returns a dictionary of string key-value pairs.</returns>
        internal Dictionary<string, string> GetAsQueryParam()
        {
            var dictionary = new Dictionary<string, string>();

            if (Id != null)
            {
                var idStringValue = Id.ToString();
                dictionary.Add("id", idStringValue);
            }

            if (Start != null)
            {
                var startStringValue = Start.ToString();
                dictionary.Add("start", startStringValue);
            }

            if (End != null)
            {
                var endStringValue = End.ToString();
                dictionary.Add("end", endStringValue);
            }

            return dictionary;
        }
    }
}
