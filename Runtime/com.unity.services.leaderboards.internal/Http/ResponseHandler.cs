using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Unity.Services.Leaderboards.Internal.Models;

namespace Unity.Services.Leaderboards.Internal.Http
{
    /// <summary>
    /// Methods for handler Response objects.
    /// </summary>
    internal static class ResponseHandler
    {
        private static List<IDeserializable> DeserializeListOfJsonObjects(List<object> objectList)
        {
            List<IDeserializable> jsonObjectList = new List<IDeserializable>();

            foreach (var o in objectList)
            {
                jsonObjectList.Add(new JsonObject(o));
            }

            return (List<IDeserializable>) (object) jsonObjectList;
        }

        /// <summary>
        /// Function for attempting to deserialize a response data to a given
        /// type.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>The data deserialized to type T.</returns>
        public static T TryDeserializeResponse<T>(HttpClientResponse response)
        {
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };

            try
            {
                var deserializedJson = GetDeserializedJson(response.Data);
                return JsonConvert.DeserializeObject<T>(deserializedJson, settings);
            }
            catch(Exception e)
            {
                throw new ResponseDeserializationException(response, e, e.Message);
            }
        }

        /// <summary>
        /// Function for attempting to deserialize a response data to a generic
        /// object.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="type">The type to cast to.</param>
        /// <returns>The data as a deserialized raw object.</returns>
        public static object TryDeserializeResponse(HttpClientResponse response, Type type)
        {
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };

            try
            {
                return JsonConvert.DeserializeObject(GetDeserializedJson(response.Data), type, settings);
            }
            catch(Exception e)
            {
                throw new ResponseDeserializationException(response, e, e.Message);
            }
        }

        private static string GetDeserializedJson(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Function for handling the response and checking for appropriate
        /// status code.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="statusCodeToTypeMap">Map of Status Codes to Types.</param>
        public static void HandleAsyncResponse(HttpClientResponse response, Dictionary<string, Type> statusCodeToTypeMap)
        {
            if (statusCodeToTypeMap.ContainsKey(response.StatusCode.ToString()))
            {
                Type responseType = statusCodeToTypeMap[response.StatusCode.ToString()];
                if (responseType != null && response.IsHttpError || response.IsNetworkError)
                {
                    if (typeof(IOneOf).IsAssignableFrom(responseType))
                    {
                        var instance = CreateOneOfException(response, responseType);
                        throw instance;
                    }
                    else
                    {
                        var instance = CreateHttpException(response, responseType);
                        throw instance;
                    }
                }
            }
            else
            {
                throw new HttpException(response);
            }
        }

        /// <summary>
        /// Function for converting a OneOf exception to an HttpException, or throwing an error if the
        /// response cannot be correctly deserialized.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="responseType">The Type of the response.</param>
        private static HttpException CreateOneOfException(HttpClientResponse response, Type responseType)
        {
            try
            {
                var dataObject = ResponseHandler.TryDeserializeResponse(response, responseType);
                return CreateHttpException(response, ((IOneOf) dataObject).Type);
            }
            catch (ArgumentException e)
            {
                throw new ResponseDeserializationException(response, e, e.Message);
            }
            catch (MissingFieldException e)
            {
                throw new ResponseDeserializationException(response, e,
                    "Discriminator field not found in the parsed json response.");
            }
            catch (ResponseDeserializationException e)
            {
                // To match the old behaviour for now, handle a
                // MissingFieldException specially.
                if (e.InnerException.GetType() == typeof(MissingFieldException))
                {
                    throw new ResponseDeserializationException(response, e.InnerException,
                        "Discriminator field not found in the parsed json response.");
                }
                if (e.response == null)
                {
                    throw new ResponseDeserializationException(response, e.Message);
                }
                throw;
            }
            catch (Exception e)
            {
                throw new ResponseDeserializationException(response, e, e.Message);
            }
        }

        /// <summary>
        /// Function for converting a response to an HttpException, or throwing an error if the
        /// response cannot be correctly deserialized.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="responseType">The Type of the response.</param>
        private static HttpException CreateHttpException(HttpClientResponse response, Type responseType)
        {
            Type exceptionType = typeof(HttpException<>);
            var genericException = exceptionType.MakeGenericType(responseType);

            try
            {
                // If the responseType is a stream, create an instance of it to return
                if (responseType == typeof(System.IO.Stream))
                {
                    var streamObject = (object)(response.Data == null ? new MemoryStream() : new MemoryStream(response.Data));
                    var streamObjectInstance = Activator.CreateInstance(genericException, new object[] {response, streamObject});
                    return (HttpException) streamObjectInstance;
                }

                // Otherwise, try to deserialize the object.
                var dataObject = ResponseHandler.TryDeserializeResponse(response, responseType);
                var instance = Activator.CreateInstance(genericException, new object[] {response, dataObject});
                return (HttpException) instance;
            }
            catch (ArgumentException e)
            {
                throw new ResponseDeserializationException(response, e, e.Message);
            }
            catch (MissingFieldException e)
            {
                throw new ResponseDeserializationException(response, e,
                    "Discriminator field not found in the parsed json response.");
            }
            catch (ResponseDeserializationException e)
            {
                if (e.response == null)
                {
                    throw new ResponseDeserializationException(response, e.Message);
                }
                throw;
            }
            catch (Exception e)
            {
                throw new ResponseDeserializationException(response, e, e.Message);
            }
        }

        /// <summary>
        /// Function for handling the response as a type and checking for
        /// appropriate status code.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="statusCodeToTypeMap">Map of Status Codes to Types.</param>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>Returns the data object as the target type.</returns>
        public static T HandleAsyncResponse<T>(HttpClientResponse response, Dictionary<string, Type> statusCodeToTypeMap) where T : class
        {
            HandleAsyncResponse(response, statusCodeToTypeMap);

            try
            {
                if (statusCodeToTypeMap[response.StatusCode.ToString()] == typeof(System.IO.Stream))
                {
                    return (response.Data == null ? new MemoryStream() : new MemoryStream(response.Data)) as T;
                }
                return ResponseHandler.TryDeserializeResponse<T>(response);
            }
            catch (ArgumentException e)
            {
                throw new ResponseDeserializationException(response, e.Message);
            }
            catch (MissingFieldException e)
            {
                throw new ResponseDeserializationException(response, e,
                    "Discriminator field not found in the parsed json response.");
            }
            catch (ResponseDeserializationException e)
            {
                if (e.response == null)
                {
                    throw new ResponseDeserializationException(response, e.Message);
                }
                throw;
            }
            catch (Exception e)
            {
                throw new ResponseDeserializationException(response, e, e.Message);
            }
        }
    }
}