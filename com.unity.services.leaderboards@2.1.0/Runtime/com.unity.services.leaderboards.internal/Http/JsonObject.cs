using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace Unity.Services.Leaderboards.Internal.Http
{
    /// <summary>
    /// JsonObject class for encapsulating generic object types. We use this to
    /// hide internal Json implementation details.
    /// </summary>
    [Preserve]
    [JsonConverter(typeof(JsonObjectConverter))]
    internal class JsonObject : IDeserializable
    {
        /// <summary>
        /// Constructor sets object as the internal object.
        /// </summary>
        [Preserve]
        internal JsonObject(object obj)
        {
            this.obj = obj;
        }

        /// <summary>The encapsulated object.</summary>
        [Preserve]
        internal object obj;

        /// <summary>
        /// Returns the internal object as a string.
        /// </summary>
        /// <returns>The internal object as a string.</returns>
        public string GetAsString()
        {
            try
            {
                if (obj == null)
                {
                    return "";
                }

                if (obj.GetType() == typeof(String))
                {
                    return obj.ToString();
                }

                return JsonConvert.SerializeObject(obj);
            }
            catch (System.Exception)
            {
                throw new InvalidOperationException("Failed to convert JsonObject to string.");
            }
        }

        /// <summary>
        /// Returns the object as a defined type.
        /// </summary>
        /// <typeparam name="T">The type to cast internal object to.</typeparam>
        /// <param name="deserializationSettings">Deserialization settings for how to handle properties like missing members.</param>
        /// <returns>The internal object case to type T.</returns>
        public T GetAs<T>(DeserializationSettings deserializationSettings = null)
        {
            // Check if deserializationSettings is null so we can use the default value.
            deserializationSettings = deserializationSettings ?? new DeserializationSettings();
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = deserializationSettings.MissingMemberHandling == MissingMemberHandling.Error
                    ? Newtonsoft.Json.MissingMemberHandling.Error
                    : Newtonsoft.Json.MissingMemberHandling.Ignore
            };
            try
            {
                var returnObject = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj), jsonSettings);
                var errors = ValidateObject(returnObject);
                if (errors.Count > 0)
                {
                    throw new DeserializationException(String.Join("\n", errors));
                }

                return returnObject;
            }
            catch (DeserializationException)
            {
                throw;
            }
            catch (JsonSerializationException e)
            {
                throw new DeserializationException(e.Message);
            }
            catch (Exception)
            {
                throw new DeserializationException("Unable to deserialize object.");
            }
        }

        /// <summary>
        /// Overload for returning the object as a defined type but without
        /// needing to specify DeserializationSettings.
        /// </summary>
        /// <typeparam name="T">The type to cast internal object to.</typeparam>
        /// <returns>The internal object case to type T.</returns>
        public T GetAs<T>()
        {
            return this.GetAs<T>(null);
        }

        /// <summary>
        /// Convert object to jsonobject.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns>The jsonobject.</returns>
        public static IDeserializable GetNewJsonObjectResponse(object o)
        {
            return (IDeserializable) new JsonObject(o);
        }

        /// <summary>
        /// Convert list of object to list of jsonobject.
        /// </summary>
        /// <param name="o">The list of objects.</param>
        /// <returns>The list of jsonobjects.</returns>
        public static List<IDeserializable> GetNewJsonObjectResponse(List<object> o)
        {
            if (o == null) {
                return null;
            }
            return o.Select(v => (IDeserializable) new JsonObject(v)).ToList();
        }

        /// <summary>
        /// Convert list of list of object to list of list of jsonobject.
        /// </summary>
        /// <param name="o">The list of list of objects.</param>
        /// <returns>The list of list of jsonobjects.</returns>
        public static List<List<IDeserializable>> GetNewJsonObjectResponse(List<List<object>> o)
        {
            if (o == null) {
                return null;
            }
            return o.Select(l => l.Select(v => v == null ? null : (IDeserializable) new JsonObject(v)).ToList()).ToList();
        }

        /// <summary>
        /// Convert dictionary of string, object to dictionary of string, jsonobject.
        /// </summary>
        /// <param name="o">The dictionary of string, objects.</param>
        /// <returns>The dictionary of string, jsonobjects.</returns>
        public static Dictionary<string, IDeserializable> GetNewJsonObjectResponse(Dictionary<string, object> o)
        {
            if (o == null) {
                return null;
            }
            return o.ToDictionary(kv => kv.Key, kv => (IDeserializable) new JsonObject(kv.Value));
        }

        /// <summary>
        /// Convert dictionary of string, list of object to dictionary of string, list of jsonobject.
        /// </summary>
        /// <typeparam name="o">The dictionary of string, list of objects.</typeparam>
        /// <returns>The dictionary of string, list of jsonobjects.</returns>
        public static Dictionary<string, List<IDeserializable>> GetNewJsonObjectResponse(Dictionary<string, List<object>> o) {
            if (o == null) {
                return null;
            }
            return o.ToDictionary(kv => kv.Key, kv => GetNewJsonObjectResponse(kv.Value));
        }

        private List<string> ValidateObject<T>(T objectToCheck, List<string> errors = null)
        {
            if (errors == null)
            {
                errors = new List<string>();
            }

            if (objectToCheck != null)
            {
                var isList = typeof(IEnumerable).IsAssignableFrom(typeof(T));
                if (isList)
                {
                    foreach (var item in (IEnumerable) objectToCheck)
                    {
                        ValidateFieldInfos(item, errors);
                        ValidatePropertyInfos(item, errors);
                    }
                }
                else
                {
                    ValidateFieldInfos(objectToCheck, errors);
                    ValidatePropertyInfos(objectToCheck, errors);
                }
            }

            return errors;
        }

        private void ValidatePropertyInfos<T>(T objectToCheck, List<string> errors)
        {
            var propertyInfos = objectToCheck.GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var value = propertyInfo.GetValue(objectToCheck);
                var memberName = propertyInfo.Name;
                var objectName = objectToCheck.GetType().Name;
                ValidateValue(value, objectName, "Property", memberName, errors);
            }
        }

        private void ValidateFieldInfos<T>(T objectToCheck, List<string> errors)
        {
            var fieldInfos = objectToCheck.GetType().GetFields();
            foreach (var fieldInfo in fieldInfos)
            {
                var value = fieldInfo.GetValue(objectToCheck);
                var memberName = fieldInfo.Name;
                var objectName = objectToCheck.GetType().Name;
                ValidateValue(value, objectName, "Field", memberName, errors);
            }
        }

        private void ValidateValue(object value, string objectName, string memberType, string memberName,
            List<string> errors)
        {
            if (!(value is ValueType) && !(value is string))
            {
                if (value is JObject)
                {
                    errors.Add(
                        $"{memberType}: \"{memberName}\" on Type: \"{objectName}\" must not be of type `object` or `dynamic`");
                }
                else
                {
                    ValidateObject(value, errors);
                }
            }
        }
    }
}
