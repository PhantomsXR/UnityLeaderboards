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
    /// LeaderboardPlayerIds model
    /// </summary>
    [Preserve]
    [DataContract(Name = "LeaderboardPlayerIds")]
    internal class LeaderboardPlayerIds
    {
        /// <summary>
        /// Creates an instance of LeaderboardPlayerIds.
        /// </summary>
        /// <param name="playerIds">playerIds param</param>
        [Preserve]
        public LeaderboardPlayerIds(List<string> playerIds = default)
        {
            PlayerIds = playerIds;
        }

        /// <summary>
        /// Parameter playerIds of LeaderboardPlayerIds
        /// </summary>
        [Preserve]
        [DataMember(Name = "playerIds", EmitDefaultValue = false)]
        public List<string> PlayerIds{ get; }

        /// <summary>
        /// Formats a LeaderboardPlayerIds into a string of key-value pairs for use as a path parameter.
        /// </summary>
        /// <returns>Returns a string representation of the key-value pairs.</returns>
        internal string SerializeAsPathParam()
        {
            var serializedModel = "";

            if (PlayerIds != null)
            {
                serializedModel += "playerIds," + PlayerIds.ToString();
            }
            return serializedModel;
        }

        /// <summary>
        /// Returns a LeaderboardPlayerIds as a dictionary of key-value pairs for use as a query parameter.
        /// </summary>
        /// <returns>Returns a dictionary of string key-value pairs.</returns>
        internal Dictionary<string, string> GetAsQueryParam()
        {
            var dictionary = new Dictionary<string, string>();

            if (PlayerIds != null)
            {
                var playerIdsStringValue = PlayerIds.ToString();
                dictionary.Add("playerIds", playerIdsStringValue);
            }

            return dictionary;
        }
    }
}
