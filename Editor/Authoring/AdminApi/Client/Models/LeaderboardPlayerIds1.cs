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
    /// LeaderboardPlayerIds1 model
    /// </summary>
    [Preserve]
    [DataContract(Name = "LeaderboardPlayerIds_1")]
    internal class LeaderboardPlayerIds1
    {
        /// <summary>
        /// Creates an instance of LeaderboardPlayerIds1.
        /// </summary>
        /// <param name="playerIds">playerIds param</param>
        [Preserve]
        public LeaderboardPlayerIds1(List<string> playerIds = default)
        {
            PlayerIds = playerIds;
        }

        /// <summary>
        /// Parameter playerIds of LeaderboardPlayerIds1
        /// </summary>
        [Preserve]
        [DataMember(Name = "playerIds", EmitDefaultValue = false)]
        public List<string> PlayerIds{ get; }
    
        /// <summary>
        /// Formats a LeaderboardPlayerIds1 into a string of key-value pairs for use as a path parameter.
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
        /// Returns a LeaderboardPlayerIds1 as a dictionary of key-value pairs for use as a query parameter.
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
