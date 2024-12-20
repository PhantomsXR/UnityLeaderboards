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
    /// PagingParams model
    /// </summary>
    [Preserve]
    [DataContract(Name = "PagingParams")]
    internal class PagingParams
    {
        /// <summary>
        /// Creates an instance of PagingParams.
        /// </summary>
        /// <param name="offset">offset param</param>
        /// <param name="limit">limit param</param>
        /// <param name="total">total param</param>
        [Preserve]
        public PagingParams(int offset = default, int limit = default, int total = default)
        {
            Offset = offset;
            Limit = limit;
            Total = total;
        }

        /// <summary>
        /// Parameter offset of PagingParams
        /// </summary>
        [Preserve]
        [DataMember(Name = "offset", EmitDefaultValue = false)]
        public int Offset{ get; }

        /// <summary>
        /// Parameter limit of PagingParams
        /// </summary>
        [Preserve]
        [DataMember(Name = "limit", EmitDefaultValue = false)]
        public int Limit{ get; }

        /// <summary>
        /// Parameter total of PagingParams
        /// </summary>
        [Preserve]
        [DataMember(Name = "total", EmitDefaultValue = false)]
        public int Total{ get; }

        /// <summary>
        /// Formats a PagingParams into a string of key-value pairs for use as a path parameter.
        /// </summary>
        /// <returns>Returns a string representation of the key-value pairs.</returns>
        internal string SerializeAsPathParam()
        {
            var serializedModel = "";

            serializedModel += "offset," + Offset.ToString() + ",";
            serializedModel += "limit," + Limit.ToString() + ",";
            serializedModel += "total," + Total.ToString();
            return serializedModel;
        }

        /// <summary>
        /// Returns a PagingParams as a dictionary of key-value pairs for use as a query parameter.
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

            return dictionary;
        }
    }
}
