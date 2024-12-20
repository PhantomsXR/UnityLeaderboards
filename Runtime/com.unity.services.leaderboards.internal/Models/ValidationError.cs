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
    /// Single error in the Validation Error Response.
    /// </summary>
    [Preserve]
    [DataContract(Name = "ValidationError")]
    internal class ValidationError
    {
        /// <summary>
        /// Single error in the Validation Error Response.
        /// </summary>
        /// <param name="field">field param</param>
        /// <param name="messages">messages param</param>
        [Preserve]
        public ValidationError(string field, List<string> messages)
        {
            Field = field;
            Messages = messages;
        }

        /// <summary>
        /// Parameter field of ValidationError
        /// </summary>
        [Preserve]
        [DataMember(Name = "field", IsRequired = true, EmitDefaultValue = true)]
        public string Field{ get; }

        /// <summary>
        /// Parameter messages of ValidationError
        /// </summary>
        [Preserve]
        [DataMember(Name = "messages", IsRequired = true, EmitDefaultValue = true)]
        public List<string> Messages{ get; }

        /// <summary>
        /// Formats a ValidationError into a string of key-value pairs for use as a path parameter.
        /// </summary>
        /// <returns>Returns a string representation of the key-value pairs.</returns>
        internal string SerializeAsPathParam()
        {
            var serializedModel = "";

            if (Field != null)
            {
                serializedModel += "field," + Field + ",";
            }
            if (Messages != null)
            {
                serializedModel += "messages," + Messages.ToString();
            }
            return serializedModel;
        }

        /// <summary>
        /// Returns a ValidationError as a dictionary of key-value pairs for use as a query parameter.
        /// </summary>
        /// <returns>Returns a dictionary of string key-value pairs.</returns>
        internal Dictionary<string, string> GetAsQueryParam()
        {
            var dictionary = new Dictionary<string, string>();

            if (Field != null)
            {
                var fieldStringValue = Field.ToString();
                dictionary.Add("field", fieldStringValue);
            }

            if (Messages != null)
            {
                var messagesStringValue = Messages.ToString();
                dictionary.Add("messages", messagesStringValue);
            }

            return dictionary;
        }
    }
}
