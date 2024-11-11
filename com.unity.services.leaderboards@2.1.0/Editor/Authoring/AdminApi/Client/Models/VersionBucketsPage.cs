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
    /// VersionBucketsPage model
    /// </summary>
    [Preserve]
    [DataContract(Name = "VersionBucketsPage")]
    internal class VersionBucketsPage
    {
        /// <summary>
        /// Creates an instance of VersionBucketsPage.
        /// </summary>
        /// <param name="offset">offset param</param>
        /// <param name="limit">limit param</param>
        /// <param name="total">total param</param>
        /// <param name="version">version param</param>
        /// <param name="results">results param</param>
        [Preserve]
        public VersionBucketsPage(int offset = default, int limit = default, int total = default, LeaderboardVersion2 version = default, List<string> results = default)
        {
            Offset = offset;
            Limit = limit;
            Total = total;
            Version = version;
            Results = results;
        }

        /// <summary>
        /// Parameter offset of VersionBucketsPage
        /// </summary>
        [Preserve]
        [DataMember(Name = "offset", EmitDefaultValue = false)]
        public int Offset{ get; }
        
        /// <summary>
        /// Parameter limit of VersionBucketsPage
        /// </summary>
        [Preserve]
        [DataMember(Name = "limit", EmitDefaultValue = false)]
        public int Limit{ get; }
        
        /// <summary>
        /// Parameter total of VersionBucketsPage
        /// </summary>
        [Preserve]
        [DataMember(Name = "total", EmitDefaultValue = false)]
        public int Total{ get; }
        
        /// <summary>
        /// Parameter version of VersionBucketsPage
        /// </summary>
        [Preserve]
        [DataMember(Name = "version", EmitDefaultValue = false)]
        public LeaderboardVersion2 Version{ get; }
        
        /// <summary>
        /// Parameter results of VersionBucketsPage
        /// </summary>
        [Preserve]
        [DataMember(Name = "results", EmitDefaultValue = false)]
        public List<string> Results{ get; }
    
        /// <summary>
        /// Formats a VersionBucketsPage into a string of key-value pairs for use as a path parameter.
        /// </summary>
        /// <returns>Returns a string representation of the key-value pairs.</returns>
        internal string SerializeAsPathParam()
        {
            var serializedModel = "";

            serializedModel += "offset," + Offset.ToString() + ",";
            serializedModel += "limit," + Limit.ToString() + ",";
            serializedModel += "total," + Total.ToString() + ",";
            if (Version != null)
            {
                serializedModel += "version," + Version.ToString() + ",";
            }
            if (Results != null)
            {
                serializedModel += "results," + Results.ToString();
            }
            return serializedModel;
        }

        /// <summary>
        /// Returns a VersionBucketsPage as a dictionary of key-value pairs for use as a query parameter.
        /// </summary>
        /// <returns>Returns a dictionary of string key-value pairs.</returns>
        internal Dictionary<string, string> GetAsQueryParam()
        {
            var dictionary = new Dictionary<string, string>();

            var offsetStringValue = Offset.ToString();
            dictionary.Add("offset", offsetStringValue);
            
            var limitStringValue = Limit.ToString();
            dictionary.Add("limit", limitStringValue);
            
            var totalStringValue = Total.ToString();
            dictionary.Add("total", totalStringValue);
            
            if (Results != null)
            {
                var resultsStringValue = Results.ToString();
                dictionary.Add("results", resultsStringValue);
            }
            
            return dictionary;
        }
    }
}
