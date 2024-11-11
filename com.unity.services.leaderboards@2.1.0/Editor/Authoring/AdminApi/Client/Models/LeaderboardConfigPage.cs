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
    /// LeaderboardConfigPage model
    /// </summary>
    [Preserve]
    [DataContract(Name = "LeaderboardConfigPage")]
    internal class LeaderboardConfigPage
    {
        /// <summary>
        /// Creates an instance of LeaderboardConfigPage.
        /// </summary>
        /// <param name="results">results param</param>
        /// <param name="pageInfo">pageInfo param</param>
        [Preserve]
        public LeaderboardConfigPage(List<UpdatedLeaderboardConfig1> results = default, PageInfo1 pageInfo = default)
        {
            Results = results;
            PageInfo = pageInfo;
        }

        /// <summary>
        /// Parameter results of LeaderboardConfigPage
        /// </summary>
        [Preserve]
        [DataMember(Name = "results", EmitDefaultValue = false)]
        public List<UpdatedLeaderboardConfig1> Results{ get; }
        
        /// <summary>
        /// Parameter pageInfo of LeaderboardConfigPage
        /// </summary>
        [Preserve]
        [DataMember(Name = "pageInfo", EmitDefaultValue = false)]
        public PageInfo1 PageInfo{ get; }
    
        /// <summary>
        /// Formats a LeaderboardConfigPage into a string of key-value pairs for use as a path parameter.
        /// </summary>
        /// <returns>Returns a string representation of the key-value pairs.</returns>
        internal string SerializeAsPathParam()
        {
            var serializedModel = "";

            if (Results != null)
            {
                serializedModel += "results," + Results.ToString() + ",";
            }
            if (PageInfo != null)
            {
                serializedModel += "pageInfo," + PageInfo.ToString();
            }
            return serializedModel;
        }

        /// <summary>
        /// Returns a LeaderboardConfigPage as a dictionary of key-value pairs for use as a query parameter.
        /// </summary>
        /// <returns>Returns a dictionary of string key-value pairs.</returns>
        internal Dictionary<string, string> GetAsQueryParam()
        {
            var dictionary = new Dictionary<string, string>();

            return dictionary;
        }
    }
}
