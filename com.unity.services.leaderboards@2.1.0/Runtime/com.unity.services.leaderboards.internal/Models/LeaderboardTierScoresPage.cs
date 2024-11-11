﻿using System;
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
    /// LeaderboardTierScoresPage model
    /// </summary>
    [Preserve]
    [DataContract(Name = "LeaderboardTierScoresPage")]
    internal class LeaderboardTierScoresPage
    {
        /// <summary>
        /// Creates an instance of LeaderboardTierScoresPage.
        /// </summary>
        /// <param name="tier">tier param</param>
        /// <param name="offset">offset param</param>
        /// <param name="limit">limit param</param>
        /// <param name="total">total param</param>
        /// <param name="results">results param</param>
        [Preserve]
        public LeaderboardTierScoresPage(string tier = default, int offset = default, int limit = default, int total = default, List<LeaderboardEntry> results = default)
        {
            Tier = tier;
            Offset = offset;
            Limit = limit;
            Total = total;
            Results = results;
        }

        /// <summary>
        /// Parameter tier of LeaderboardTierScoresPage
        /// </summary>
        [Preserve]
        [DataMember(Name = "tier", EmitDefaultValue = false)]
        public string Tier{ get; }

        /// <summary>
        /// Parameter offset of LeaderboardTierScoresPage
        /// </summary>
        [Preserve]
        [DataMember(Name = "offset", EmitDefaultValue = false)]
        public int Offset{ get; }

        /// <summary>
        /// Parameter limit of LeaderboardTierScoresPage
        /// </summary>
        [Preserve]
        [DataMember(Name = "limit", EmitDefaultValue = false)]
        public int Limit{ get; }

        /// <summary>
        /// Parameter total of LeaderboardTierScoresPage
        /// </summary>
        [Preserve]
        [DataMember(Name = "total", EmitDefaultValue = false)]
        public int Total{ get; }

        /// <summary>
        /// Parameter results of LeaderboardTierScoresPage
        /// </summary>
        [Preserve]
        [DataMember(Name = "results", EmitDefaultValue = false)]
        public List<LeaderboardEntry> Results{ get; }

        /// <summary>
        /// Formats a LeaderboardTierScoresPage into a string of key-value pairs for use as a path parameter.
        /// </summary>
        /// <returns>Returns a string representation of the key-value pairs.</returns>
        internal string SerializeAsPathParam()
        {
            var serializedModel = "";

            if (Tier != null)
            {
                serializedModel += "tier," + Tier + ",";
            }
            serializedModel += "offset," + Offset.ToString() + ",";
            serializedModel += "limit," + Limit.ToString() + ",";
            serializedModel += "total," + Total.ToString() + ",";
            if (Results != null)
            {
                serializedModel += "results," + Results.ToString();
            }
            return serializedModel;
        }

        /// <summary>
        /// Returns a LeaderboardTierScoresPage as a dictionary of key-value pairs for use as a query parameter.
        /// </summary>
        /// <returns>Returns a dictionary of string key-value pairs.</returns>
        internal Dictionary<string, string> GetAsQueryParam()
        {
            var dictionary = new Dictionary<string, string>();

            if (Tier != null)
            {
                var tierStringValue = Tier.ToString();
                dictionary.Add("tier", tierStringValue);
            }

            var offsetStringValue = Offset.ToString();
            dictionary.Add("offset", offsetStringValue);

            var limitStringValue = Limit.ToString();
            dictionary.Add("limit", limitStringValue);

            var totalStringValue = Total.ToString();
            dictionary.Add("total", totalStringValue);

            return dictionary;
        }
    }
}
