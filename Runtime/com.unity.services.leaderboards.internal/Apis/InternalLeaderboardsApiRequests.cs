using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using Unity.Services.Leaderboards.Internal.Models;
using Unity.Services.Leaderboards.Internal.Scheduler;
using Unity.Services.Authentication.Internal;

namespace Unity.Services.Leaderboards.Internal.Leaderboards
{
    internal static class JsonSerialization
    {
        public static byte[] Serialize<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(SerializeToString(obj));
        }

        public static string SerializeToString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings {ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore});
        }
    }

    /// <summary>
    /// LeaderboardsApiBaseRequest class
    /// </summary>
    [Preserve]
    internal class LeaderboardsApiBaseRequest
    {
        /// <summary>
        /// Helper function to add a provided key and value to the provided
        /// query params and to escape the values correctly if it is a URL.
        /// </summary>
        /// <param name="queryParams">A `List/<string/>` of the query parameters.</param>
        /// <param name="key">The key to be added.</param>
        /// <param name="value">The value to be added.</param>
        /// <returns>Returns a `List/<string/>` with the `key` and `value` added to the provided `queryParams`.</returns>
        [Preserve]
        public List<string> AddParamsToQueryParams(List<string> queryParams, string key, string value)
        {
            key = UnityWebRequest.EscapeURL(key);
            value = UnityWebRequest.EscapeURL(value);
            queryParams.Add($"{key}={value}");

            return queryParams;
        }

        /// <summary>
        /// Helper function to add a provided key and list of values to the
        /// provided query params and to escape the values correctly if it is a
        /// URL.
        /// </summary>
        /// <param name="queryParams">A `List/<string/>` of the query parameters.</param>
        /// <param name="key">The key to be added.</param>
        /// <param name="values">List of values to be added.</param>
        /// <param name="style">string for defining the style, currently unused.</param>
        /// <param name="explode">True if query params should be escaped and added separately.</param>
        /// <returns>Returns a `List/<string/>`</returns>
        [Preserve]
        public List<string> AddParamsToQueryParams(List<string> queryParams, string key, List<string> values, string style, bool explode)
        {
            if (explode)
            {
                foreach (var value in values)
                {
                    string escapedValue = UnityWebRequest.EscapeURL(value);
                    queryParams.Add($"{UnityWebRequest.EscapeURL(key)}={escapedValue}");
                }
            }
            else
            {
                string paramString = $"{UnityWebRequest.EscapeURL(key)}=";
                foreach (var value in values)
                {
                    paramString += UnityWebRequest.EscapeURL(value) + ",";
                }
                paramString = paramString.Remove(paramString.Length - 1);
                queryParams.Add(paramString);
            }

            return queryParams;
        }

        /// <summary>
        /// Helper function to add a provided map of keys and values, representing a model, to the
        /// provided query params.
        /// </summary>
        /// <param name="queryParams">A `List/<string/>` of the query parameters.</param>
        /// <param name="modelVars">A `Dictionary` representing the vars of the model</param>
        /// <returns>Returns a `List/<string/>`</returns>
        [Preserve]
        public List<string> AddParamsToQueryParams(List<string> queryParams, Dictionary<string, string> modelVars)
        {
            foreach (var key in modelVars.Keys)
            {
                string escapedValue = UnityWebRequest.EscapeURL(modelVars[key]);
                queryParams.Add($"{UnityWebRequest.EscapeURL(key)}={escapedValue}");
            }

            return queryParams;
        }

        /// <summary>
        /// Helper function to add a provided key and value to the provided
        /// query params and to escape the values correctly if it is a URL.
        /// </summary>
        /// <param name="queryParams">A `List/<string/>` of the query parameters.</param>
        /// <param name="key">The key to be added.</param>
        /// <typeparam name="T">The type of the value to be added.</typeparam>
        /// <param name="value">The value to be added.</param>
        /// <returns>Returns a `List/<string/>`</returns>
        [Preserve]
        public List<string> AddParamsToQueryParams<T>(List<string> queryParams, string key, T value)
        {
            if (queryParams == null)
            {
                queryParams = new List<string>();
            }

            key = UnityWebRequest.EscapeURL(key);
            string valueString = UnityWebRequest.EscapeURL(value.ToString());
            queryParams.Add($"{key}={valueString}");
            return queryParams;
        }

        /// <summary>
        /// Constructs a string representing an array path parameter.
        /// </summary>
        /// <param name="pathParam">The list of values to convert to string.</param>
        /// <returns>String representing the param.</returns>
        [Preserve]
        public string GetPathParamString(List<string> pathParam)
        {
            string paramString = "";
            foreach (var value in pathParam)
            {
                paramString += UnityWebRequest.EscapeURL(value) + ",";
            }
            paramString = paramString.Remove(paramString.Length - 1);
            return paramString;
        }

        /// <summary>
        /// Constructs the body of the request based on IO stream.
        /// </summary>
        /// <param name="stream">The IO stream to use.</param>
        /// <returns>Byte array representing the body.</returns>
        public byte[] ConstructBody(System.IO.Stream stream)
        {
            if (stream != null)
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// Construct the request body based on string value.
        /// </summary>
        /// <param name="s">The input body.</param>
        /// <returns>Byte array representing the body.</returns>
        public byte[] ConstructBody(string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        /// <summary>
        /// Construct request body based on generic object.
        /// </summary>
        /// <param name="o">The object to use.</param>
        /// <returns>Byte array representing the body.</returns>
        public byte[] ConstructBody(object o)
        {
            return JsonSerialization.Serialize(o);
        }

        /// <summary>
        /// Generate an accept header.
        /// </summary>
        /// <param name="accepts">list of accepts objects.</param>
        /// <returns>The generated accept header.</returns>
        public string GenerateAcceptHeader(string[] accepts)
        {
            if (accepts.Length == 0)
            {
                return null;
            }
            for (int i = 0; i < accepts.Length; ++i)
            {
                if (string.Equals(accepts[i], "application/json", System.StringComparison.OrdinalIgnoreCase))
                {
                    return "application/json";
                }
            }
            return string.Join(", ", accepts);
        }

        private static readonly Regex JsonRegex = new Regex(@"application\/json(;\s)?((charset=utf8|q=[0-1]\.\d)(\s)?)*");

        /// <summary>
        /// Generate Content Type Header.
        /// </summary>
        /// <param name="contentTypes">The content types.</param>
        /// <returns>The Content Type Header.</returns>
        public string GenerateContentTypeHeader(string[] contentTypes)
        {
            if (contentTypes.Length == 0)
            {
                return null;
            }

            for (int i = 0; i < contentTypes.Length; ++i)
            {
                if (!string.IsNullOrWhiteSpace(contentTypes[i]) && JsonRegex.IsMatch(contentTypes[i]))
                {
                    return contentTypes[i];
                }
            }
            return contentTypes[0];
        }

        /// <summary>
        /// Generate multipart form file section.
        /// </summary>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="stream">The file stream to use.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>Returns a multipart form section.</returns>
        public IMultipartFormSection GenerateMultipartFormFileSection(string paramName, System.IO.FileStream stream, string contentType)
        {
            return new MultipartFormFileSection(paramName, ConstructBody(stream), GetFileName(stream.Name), contentType);
        }

        /// <summary>
        /// Generate multipart form file section.
        /// </summary>
        /// <param name="paramName">The parameter name.</param>
        /// <param name="stream">The IO stream to use.</param>
        /// <param name="contentType">The content type.</param>
        /// <returns>Returns a multipart form section.</returns>
        public IMultipartFormSection GenerateMultipartFormFileSection(string paramName, System.IO.Stream stream, string contentType)
        {
            return new MultipartFormFileSection(paramName, ConstructBody(stream), Guid.NewGuid().ToString(), contentType);
        }

        private string GetFileName(string filePath)
        {
            return System.IO.Path.GetFileName(filePath);
        }
    }

    /// <summary>
    /// AddLeaderboardPlayerScoreRequest
    /// Add Leaderboard Player Score
    /// </summary>
    [Preserve]
    internal class AddLeaderboardPlayerScoreRequest : LeaderboardsApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for leaderboardId </summary>
        [Preserve]
        public string LeaderboardId { get; }
        /// <summary>Accessor for playerId </summary>
        [Preserve]
        public string PlayerId { get; }
        /// <summary>Accessor for addLeaderboardScore </summary>
        [Preserve]
        public Unity.Services.Leaderboards.Internal.Models.AddLeaderboardScore AddLeaderboardScore { get; }
        string PathAndQueryParams;

        /// <summary>
        /// AddLeaderboardPlayerScore Request Object.
        /// Add Leaderboard Player Score
        /// </summary>
        /// <param name="projectId">The project's [Project ID](https://docs.unity.com/ugs-overview/ManagingUnityProjects.html)</param>
        /// <param name="leaderboardId">ID of the leaderboard</param>
        /// <param name="playerId">ID of the player</param>
        /// <param name="addLeaderboardScore">AddLeaderboardScore param</param>
        [Preserve]
        public AddLeaderboardPlayerScoreRequest(string projectId, string leaderboardId, string playerId, Unity.Services.Leaderboards.Internal.Models.AddLeaderboardScore addLeaderboardScore = default(Unity.Services.Leaderboards.Internal.Models.AddLeaderboardScore))
        {
            ProjectId = projectId;
            LeaderboardId = leaderboardId;
            PlayerId = playerId;
            AddLeaderboardScore = addLeaderboardScore;
            PathAndQueryParams = $"/v1/projects/{projectId}/leaderboards/{leaderboardId}/scores/players/{playerId}";
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            if (AddLeaderboardScore != null)
            {
                return ConstructBody(AddLeaderboardScore);
            }
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="accessToken">The auth access token to use.</param>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken,
            Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accessToken.AccessToken))
            {
                headers.Add("authorization", "Bearer " + accessToken.AccessToken);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes =
            {
                "application/json"
            };

            string[] accepts =
            {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "POST";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetLeaderboardPlayerRangeRequest
    /// Get Player Range
    /// </summary>
    [Preserve]
    internal class GetLeaderboardPlayerRangeRequest : LeaderboardsApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for leaderboardId </summary>
        [Preserve]
        public string LeaderboardId { get; }
        /// <summary>Accessor for playerId </summary>
        [Preserve]
        public string PlayerId { get; }
        /// <summary>Accessor for rangeLimit </summary>
        [Preserve]
        public int? RangeLimit { get; }
        /// <summary>Accessor for includeMetadata </summary>
        [Preserve]
        public bool? IncludeMetadata { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetLeaderboardPlayerRange Request Object.
        /// Get Player Range
        /// </summary>
        /// <param name="projectId">The project's [Project ID](https://docs.unity.com/ugs-overview/ManagingUnityProjects.html)</param>
        /// <param name="leaderboardId">ID of the leaderboard</param>
        /// <param name="playerId">ID of the player</param>
        /// <param name="rangeLimit">The number of entries either side of the player to retrieve. Defaults to 5.</param
        /// <param name="includeMetadata">If true, include any metadata for entries. Defaults to false.</param>
        [Preserve]
        public GetLeaderboardPlayerRangeRequest(string projectId, string leaderboardId, string playerId, int? rangeLimit = default(int?), bool? includeMetadata = false)
        {
            ProjectId = projectId;
            LeaderboardId = leaderboardId;
            PlayerId = playerId;
            RangeLimit = rangeLimit;
            IncludeMetadata = includeMetadata;

            PathAndQueryParams = $"/v1/projects/{projectId}/leaderboards/{leaderboardId}/scores/players/{playerId}/range";

            List<string> queryParams = new List<string>();


            var rangeLimitStringValue = RangeLimit.ToString();
            queryParams = AddParamsToQueryParams(queryParams, "rangeLimit", rangeLimitStringValue);
            if (includeMetadata == true)
            {
                queryParams = AddParamsToQueryParams(queryParams, "includeMetadata", includeMetadata.ToString());
            }
            if (queryParams.Count > 0)
            {
                PathAndQueryParams = $"{PathAndQueryParams}?{string.Join("&", queryParams)}";
            }
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="accessToken">The auth access token to use.</param>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken,
            Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accessToken.AccessToken))
            {
                headers.Add("authorization", "Bearer " + accessToken.AccessToken);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes =
            {
            };

            string[] accepts =
            {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "GET";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetLeaderboardScoresByTierRequest
    /// Get Leaderboard Scores By Tier
    /// </summary>
    [Preserve]
    internal class GetLeaderboardScoresByTierRequest : LeaderboardsApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for leaderboardId </summary>
        [Preserve]
        public string LeaderboardId { get; }
        /// <summary>Accessor for tierId </summary>
        [Preserve]
        public string TierId { get; }
        /// <summary>Accessor for offset </summary>
        [Preserve]
        public int? Offset { get; }
        /// <summary>Accessor for limit </summary>
        [Preserve]
        public int? Limit { get; }
        /// <summary>Accessor for includeMetadata </summary>
        [Preserve]
        public bool? IncludeMetadata { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetLeaderboardScoresByTier Request Object.
        /// Get Leaderboard Scores By Tier
        /// </summary>
        /// <param name="projectId">The project's [Project ID](https://docs.unity.com/ugs-overview/ManagingUnityProjects.html)</param>
        /// <param name="leaderboardId">ID of the leaderboard</param>
        /// <param name="tierId">ID of the leaderboard tier.</param>
        /// <param name="offset">The number of entries to skip when retrieving the leaderboard scores. Defaults to 0</param>
        /// <param name="limit">The number of leaderboard scores to return. Defaults to 10</param>
        /// <param name="includeMetadata">If true, include any metadata for entries. Defaults to false.</param>
        [Preserve]
        public GetLeaderboardScoresByTierRequest(string projectId, string leaderboardId, string tierId, int? offset = default(int?), int? limit = default(int?), bool? includeMetadata = false)
        {
            ProjectId = projectId;
            LeaderboardId = leaderboardId;
            TierId = tierId;
            Offset = offset;
            Limit = limit;
            IncludeMetadata = includeMetadata;

            PathAndQueryParams = $"/v1/projects/{projectId}/leaderboards/{leaderboardId}/tiers/{tierId}/scores";

            List<string> queryParams = new List<string>();


            var offsetStringValue = Offset.ToString();
            queryParams = AddParamsToQueryParams(queryParams, "offset", offsetStringValue);

            var limitStringValue = Limit.ToString();
            queryParams = AddParamsToQueryParams(queryParams, "limit", limitStringValue);
            if (includeMetadata == true)
            {
                queryParams = AddParamsToQueryParams(queryParams, "includeMetadata", includeMetadata.ToString());
            }
            if (queryParams.Count > 0)
            {
                PathAndQueryParams = $"{PathAndQueryParams}?{string.Join("&", queryParams)}";
            }
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="accessToken">The auth access token to use.</param>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken,
            Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accessToken.AccessToken))
            {
                headers.Add("authorization", "Bearer " + accessToken.AccessToken);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes =
            {
            };

            string[] accepts =
            {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "GET";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetLeaderboardPlayerScoreRequest
    /// Get Leaderboard Player Score
    /// </summary>
    [Preserve]
    internal class GetLeaderboardPlayerScoreRequest : LeaderboardsApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for leaderboardId </summary>
        [Preserve]
        public string LeaderboardId { get; }
        /// <summary>Accessor for playerId </summary>
        [Preserve]
        public string PlayerId { get; }
        /// <summary>Accessor for includeMetadata </summary>
        [Preserve]
        public bool? IncludeMetadata { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetLeaderboardPlayerScore Request Object.
        /// Get Leaderboard Player Score
        /// </summary>
        /// <param name="projectId">The project's [Project ID](https://docs.unity.com/ugs-overview/ManagingUnityProjects.html)</param>
        /// <param name="leaderboardId">ID of the leaderboard</param>
        /// <param name="playerId">ID of the player</param>
        /// <param name="includeMetadata">If true, include any metadata for entries. Defaults to false.</param>
        [Preserve]
        public GetLeaderboardPlayerScoreRequest(string projectId, string leaderboardId, string playerId, bool? includeMetadata = false)
        {
            ProjectId = projectId;
            LeaderboardId = leaderboardId;
            PlayerId = playerId;
            IncludeMetadata = includeMetadata;

            PathAndQueryParams = $"/v1/projects/{projectId}/leaderboards/{leaderboardId}/scores/players/{playerId}";

            List<string> queryParams = new List<string>();
            if (includeMetadata == true)
            {
                queryParams = AddParamsToQueryParams(queryParams, "includeMetadata", includeMetadata.ToString());
            }
            if (queryParams.Count > 0)
            {
                PathAndQueryParams = $"{PathAndQueryParams}?{string.Join("&", queryParams)}";
            }
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="accessToken">The auth access token to use.</param>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken,
            Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accessToken.AccessToken))
            {
                headers.Add("authorization", "Bearer " + accessToken.AccessToken);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes =
            {
            };

            string[] accepts =
            {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "GET";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetLeaderboardScoresByPlayerIdsRequest
    /// Get Scores By PlayerIds
    /// </summary>
    [Preserve]
    internal class GetLeaderboardScoresByPlayerIdsRequest : LeaderboardsApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for leaderboardId </summary>
        [Preserve]
        public string LeaderboardId { get; }
        /// <summary>Accessor for leaderboardPlayerIds </summary>
        [Preserve]
        public Unity.Services.Leaderboards.Internal.Models.LeaderboardPlayerIds LeaderboardPlayerIds { get; }
        /// <summary>Accessor for includeMetadata </summary>
        [Preserve]
        public bool? IncludeMetadata { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetLeaderboardScoresByPlayerIds Request Object.
        /// Get Scores By PlayerIds
        /// </summary>
        /// <param name="projectId">The project's [Project ID](https://docs.unity.com/ugs-overview/ManagingUnityProjects.html)</param>
        /// <param name="leaderboardId">ID of the leaderboard</param>
        /// <param name="leaderboardPlayerIds">LeaderboardPlayerIds param</param>
        /// <param name="includeMetadata">If true, include any metadata for entries. Defaults to false.</param>
        [Preserve]
        public GetLeaderboardScoresByPlayerIdsRequest(string projectId, string leaderboardId, Unity.Services.Leaderboards.Internal.Models.LeaderboardPlayerIds leaderboardPlayerIds = default(Unity.Services.Leaderboards.Internal.Models.LeaderboardPlayerIds), bool? includeMetadata = false)
        {
            ProjectId = projectId;
            LeaderboardId = leaderboardId;
            LeaderboardPlayerIds = leaderboardPlayerIds;
            IncludeMetadata = includeMetadata;

            PathAndQueryParams = $"/v1/projects/{projectId}/leaderboards/{leaderboardId}/scores/players";

            List<string> queryParams = new List<string>();
            if (includeMetadata == true)
            {
                queryParams = AddParamsToQueryParams(queryParams, "includeMetadata", includeMetadata.ToString());
            }
            if (queryParams.Count > 0)
            {
                PathAndQueryParams = $"{PathAndQueryParams}?{string.Join("&", queryParams)}";
            }
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            if (LeaderboardPlayerIds != null)
            {
                return ConstructBody(LeaderboardPlayerIds);
            }
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="accessToken">The auth access token to use.</param>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken,
            Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accessToken.AccessToken))
            {
                headers.Add("authorization", "Bearer " + accessToken.AccessToken);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes =
            {
                "application/json"
            };

            string[] accepts =
            {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "POST";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetLeaderboardScoresRequest
    /// Get Leaderboard Scores
    /// </summary>
    [Preserve]
    internal class GetLeaderboardScoresRequest : LeaderboardsApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for leaderboardId </summary>
        [Preserve]
        public string LeaderboardId { get; }
        /// <summary>Accessor for offset </summary>
        [Preserve]
        public int? Offset { get; }
        /// <summary>Accessor for limit </summary>
        [Preserve]
        public int? Limit { get; }
        /// <summary>Accessor for includeMetadata </summary>
        [Preserve]
        public bool? IncludeMetadata { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetLeaderboardScores Request Object.
        /// Get Leaderboard Scores
        /// </summary>
        /// <param name="projectId">The project's [Project ID](https://docs.unity.com/ugs-overview/ManagingUnityProjects.html)</param>
        /// <param name="leaderboardId">ID of the leaderboard</param>
        /// <param name="offset">The number of entries to skip when retrieving the Leaderboard scores. Defaults to 0</param>
        /// <param name="limit">The number of leaderboard scores to return. Defaults to 10</param>
        /// <param name="includeMetadata">If true, include any metadata for entries. Defaults to false.</param>
        [Preserve]
        public GetLeaderboardScoresRequest(string projectId, string leaderboardId, int? offset = default(int?), int? limit = default(int?), bool? includeMetadata = false)
        {
            ProjectId = projectId;
            LeaderboardId = leaderboardId;
            Offset = offset;
            Limit = limit;
            IncludeMetadata = includeMetadata;

            PathAndQueryParams = $"/v1/projects/{projectId}/leaderboards/{leaderboardId}/scores";

            List<string> queryParams = new List<string>();


            var offsetStringValue = Offset.ToString();
            queryParams = AddParamsToQueryParams(queryParams, "offset", offsetStringValue);

            var limitStringValue = Limit.ToString();
            queryParams = AddParamsToQueryParams(queryParams, "limit", limitStringValue);
            if (includeMetadata == true)
            {
                queryParams = AddParamsToQueryParams(queryParams, "includeMetadata", includeMetadata.ToString());
            }
            if (queryParams.Count > 0)
            {
                PathAndQueryParams = $"{PathAndQueryParams}?{string.Join("&", queryParams)}";
            }
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="accessToken">The auth access token to use.</param>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken,
            Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accessToken.AccessToken))
            {
                headers.Add("authorization", "Bearer " + accessToken.AccessToken);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes =
            {
            };

            string[] accepts =
            {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "GET";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetLeaderboardVersionPlayerScoreRequest
    /// Get Archived Leaderboard Version Player Score
    /// </summary>
    [Preserve]
    internal class GetLeaderboardVersionPlayerScoreRequest : LeaderboardsApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for leaderboardId </summary>
        [Preserve]
        public string LeaderboardId { get; }
        /// <summary>Accessor for versionId </summary>
        [Preserve]
        public string VersionId { get; }
        /// <summary>Accessor for playerId </summary>
        [Preserve]
        public string PlayerId { get; }
        /// <summary>Accessor for includeMetadata </summary>
        [Preserve]
        public bool? IncludeMetadata { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetLeaderboardVersionPlayerScore Request Object.
        /// Get Archived Leaderboard Version Player Score
        /// </summary>
        /// <param name="projectId">The project's [Project ID](https://docs.unity.com/ugs-overview/ManagingUnityProjects.html)</param>
        /// <param name="leaderboardId">ID of the leaderboard</param>
        /// <param name="versionId">ID of the leaderboard version</param>
        /// <param name="playerId">ID of the player</param>
        /// <param name="includeMetadata">If true, include any metadata for entries. Defaults to false.</param>
        [Preserve]
        public GetLeaderboardVersionPlayerScoreRequest(string projectId, string leaderboardId, string versionId, string playerId, bool? includeMetadata = false)
        {
            ProjectId = projectId;
            LeaderboardId = leaderboardId;
            VersionId = versionId;
            PlayerId = playerId;
            IncludeMetadata = includeMetadata;

            PathAndQueryParams = $"/v1/projects/{projectId}/leaderboards/{leaderboardId}/versions/{versionId}/scores/players/{playerId}";

            List<string> queryParams = new List<string>();
            if (includeMetadata == true)
            {
                queryParams = AddParamsToQueryParams(queryParams, "includeMetadata", includeMetadata.ToString());
            }
            if (queryParams.Count > 0)
            {
                PathAndQueryParams = $"{PathAndQueryParams}?{string.Join("&", queryParams)}";
            }
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="accessToken">The auth access token to use.</param>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken,
            Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accessToken.AccessToken))
            {
                headers.Add("authorization", "Bearer " + accessToken.AccessToken);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes =
            {
            };

            string[] accepts =
            {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "GET";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetLeaderboardVersionScoresRequest
    /// Get Archived Leaderboard Version Scores
    /// </summary>
    [Preserve]
    internal class GetLeaderboardVersionScoresRequest : LeaderboardsApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for leaderboardId </summary>
        [Preserve]
        public string LeaderboardId { get; }
        /// <summary>Accessor for versionId </summary>
        [Preserve]
        public string VersionId { get; }
        /// <summary>Accessor for offset </summary>
        [Preserve]
        public int? Offset { get; }
        /// <summary>Accessor for limit </summary>
        [Preserve]
        public int? Limit { get; }
        /// <summary>Accessor for includeMetadata </summary>
        [Preserve]
        public bool? IncludeMetadata { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetLeaderboardVersionScores Request Object.
        /// Get Archived Leaderboard Version Scores
        /// </summary>
        /// <param name="projectId">The project's [Project ID](https://docs.unity.com/ugs-overview/ManagingUnityProjects.html)</param>
        /// <param name="leaderboardId">ID of the leaderboard</param>
        /// <param name="versionId">ID of the leaderboard version</param>
        /// <param name="offset">The number of entries to skip when retrieving the Leaderboard scores. Defaults to 0</param>
        /// <param name="limit">The number of leaderboard scores to return. Defaults to 10</param>
        /// <param name="includeMetadata">If true, include any metadata for entries. Defaults to false.</param>
        [Preserve]
        public GetLeaderboardVersionScoresRequest(string projectId, string leaderboardId, string versionId, int? offset = default(int?), int? limit = default(int?), bool? includeMetadata = false)
        {
            ProjectId = projectId;
            LeaderboardId = leaderboardId;
            VersionId = versionId;
            Offset = offset;
            Limit = limit;
            IncludeMetadata = includeMetadata;

            PathAndQueryParams = $"/v1/projects/{projectId}/leaderboards/{leaderboardId}/versions/{versionId}/scores";

            List<string> queryParams = new List<string>();


            var offsetStringValue = Offset.ToString();
            queryParams = AddParamsToQueryParams(queryParams, "offset", offsetStringValue);

            var limitStringValue = Limit.ToString();
            queryParams = AddParamsToQueryParams(queryParams, "limit", limitStringValue);
            if (includeMetadata == true)
            {
                queryParams = AddParamsToQueryParams(queryParams, "includeMetadata", includeMetadata.ToString());
            }
            if (queryParams.Count > 0)
            {
                PathAndQueryParams = $"{PathAndQueryParams}?{string.Join("&", queryParams)}";
            }
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="accessToken">The auth access token to use.</param>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken,
            Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accessToken.AccessToken))
            {
                headers.Add("authorization", "Bearer " + accessToken.AccessToken);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes =
            {
            };

            string[] accepts =
            {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "GET";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetLeaderboardVersionPlayerRangeRequest
    /// Get Player Range for Archived Leaderboard
    /// </summary>
    [Preserve]
    internal class GetLeaderboardVersionPlayerRangeRequest : LeaderboardsApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for leaderboardId </summary>
        [Preserve]
        public string LeaderboardId { get; }
        /// <summary>Accessor for versionId </summary>
        [Preserve]
        public string VersionId { get; }
        /// <summary>Accessor for playerId </summary>
        [Preserve]
        public string PlayerId { get; }
        /// <summary>Accessor for rangeLimit </summary>
        [Preserve]
        public int? RangeLimit { get; }
        /// <summary>Accessor for includeMetadata </summary>
        [Preserve]
        public bool? IncludeMetadata { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetLeaderboardVersionPlayerRange Request Object.
        /// Get Player Range for Archived Leaderboard
        /// </summary>
        /// <param name="projectId">The project's [Project ID](https://docs.unity.com/ugs-overview/ManagingUnityProjects.html)</param>
        /// <param name="leaderboardId">ID of the leaderboard</param>
        /// <param name="versionId">ID of the leaderboard version</param>
        /// <param name="playerId">ID of the player</param>
        /// <param name="rangeLimit">The number of entries either side of the player to retrieve. Defaults to 5.</param>
        /// <param name="includeMetadata">If true, include any metadata for entries. Defaults to false.</param>
        [Preserve]
        public GetLeaderboardVersionPlayerRangeRequest(string projectId, string leaderboardId, string versionId, string playerId, int? rangeLimit = default(int?), bool? includeMetadata = false)
        {
            ProjectId = projectId;
            LeaderboardId = leaderboardId;
            VersionId = versionId;
            PlayerId = playerId;
            RangeLimit = rangeLimit;
            IncludeMetadata = includeMetadata;

            PathAndQueryParams = $"/v1/projects/{projectId}/leaderboards/{leaderboardId}/versions/{versionId}/scores/players/{playerId}/range";

            List<string> queryParams = new List<string>();


            var rangeLimitStringValue = RangeLimit.ToString();
            queryParams = AddParamsToQueryParams(queryParams, "rangeLimit", rangeLimitStringValue);
            if (includeMetadata == true)
            {
                queryParams = AddParamsToQueryParams(queryParams, "includeMetadata", includeMetadata.ToString());
            }
            if (queryParams.Count > 0)
            {
                PathAndQueryParams = $"{PathAndQueryParams}?{string.Join("&", queryParams)}";
            }
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="accessToken">The auth access token to use.</param>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken,
            Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accessToken.AccessToken))
            {
                headers.Add("authorization", "Bearer " + accessToken.AccessToken);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes =
            {
            };

            string[] accepts =
            {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "GET";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetLeaderboardScoresByPlayerIdsArchiveVersionRequest
    /// Get Scores By PlayerIds for Archived Leaderboard
    /// </summary>
    [Preserve]
    internal class GetLeaderboardVersionScoresByPlayerIdsRequest : LeaderboardsApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for leaderboardId </summary>
        [Preserve]
        public string LeaderboardId { get; }
        /// <summary>Accessor for versionId </summary>
        [Preserve]
        public string VersionId { get; }
        /// <summary>Accessor for leaderboardPlayerIds </summary>
        [Preserve]
        public Unity.Services.Leaderboards.Internal.Models.LeaderboardPlayerIds LeaderboardPlayerIds { get; }
        /// <summary>Accessor for includeMetadata </summary>
        [Preserve]
        public bool? IncludeMetadata { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetLeaderboardScoresByPlayerIdsArchiveVersion Request Object.
        /// Get Scores By PlayerIds for Archived Leaderboard
        /// </summary>
        /// <param name="projectId">The project's [Project ID](https://docs.unity.com/ugs-overview/ManagingUnityProjects.html)</param>
        /// <param name="leaderboardId">ID of the leaderboard</param>
        /// <param name="versionId">ID of the leaderboard version</param>
        /// <param name="leaderboardPlayerIds">LeaderboardPlayerIds param</param>
        /// <param name="includeMetadata">If true, include any metadata for entries. Defaults to false.</param>
        [Preserve]
        public GetLeaderboardVersionScoresByPlayerIdsRequest(string projectId, string leaderboardId, string versionId, Unity.Services.Leaderboards.Internal.Models.LeaderboardPlayerIds leaderboardPlayerIds = default(Unity.Services.Leaderboards.Internal.Models.LeaderboardPlayerIds), bool? includeMetadata = false)
        {
            ProjectId = projectId;
            LeaderboardId = leaderboardId;
            VersionId = versionId;
            LeaderboardPlayerIds = leaderboardPlayerIds;
            IncludeMetadata = includeMetadata;

            PathAndQueryParams = $"/v1/projects/{projectId}/leaderboards/{leaderboardId}/versions/{versionId}/scores/players";

            List<string> queryParams = new List<string>();
            if (includeMetadata == true)
            {
                queryParams = AddParamsToQueryParams(queryParams, "includeMetadata", includeMetadata.ToString());
            }
            if (queryParams.Count > 0)
            {
                PathAndQueryParams = $"{PathAndQueryParams}?{string.Join("&", queryParams)}";
            }
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            if (LeaderboardPlayerIds != null)
            {
                return ConstructBody(LeaderboardPlayerIds);
            }
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="accessToken">The auth access token to use.</param>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken,
            Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accessToken.AccessToken))
            {
                headers.Add("authorization", "Bearer " + accessToken.AccessToken);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes =
            {
                "application/json"
            };

            string[] accepts =
            {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "POST";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetLeaderboardVersionScoresByTierRequest
    /// Get Archived Leaderboard Version Scores By Tier
    /// </summary>
    [Preserve]
    internal class GetLeaderboardVersionScoresByTierRequest : LeaderboardsApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for leaderboardId </summary>
        [Preserve]
        public string LeaderboardId { get; }
        /// <summary>Accessor for versionId </summary>
        [Preserve]
        public string VersionId { get; }
        /// <summary>Accessor for tierId </summary>
        [Preserve]
        public string TierId { get; }
        /// <summary>Accessor for offset </summary>
        [Preserve]
        public int? Offset { get; }
        /// <summary>Accessor for limit </summary>
        [Preserve]
        public int? Limit { get; }
        /// <summary>Accessor for includeMetadata </summary>
        [Preserve]
        public bool? IncludeMetadata { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetLeaderboardVersionScoresByTier Request Object.
        /// Get Archived Leaderboard Version Scores By Tier
        /// </summary>
        /// <param name="projectId">The project's [Project ID](https://docs.unity.com/ugs-overview/ManagingUnityProjects.html)</param>
        /// <param name="leaderboardId">ID of the leaderboard</param>
        /// <param name="versionId">ID of the leaderboard version</param>
        /// <param name="tierId">ID of the leaderboard tier.</param>
        /// <param name="offset">The number of entries to skip when retrieving the leaderboard scores. Defaults to 0</param>
        /// <param name="limit">The number of leaderboard scores to return. Defaults to 10</param>
        /// <param name="includeMetadata">If true, include any metadata for entries. Defaults to false.</param>
        [Preserve]
        public GetLeaderboardVersionScoresByTierRequest(string projectId, string leaderboardId, string versionId, string tierId, int? offset = default(int?), int? limit = default(int?), bool? includeMetadata = false)
        {
            ProjectId = projectId;
            LeaderboardId = leaderboardId;
            VersionId = versionId;
            TierId = tierId;
            Offset = offset;
            Limit = limit;
            IncludeMetadata = includeMetadata;

            PathAndQueryParams = $"/v1/projects/{projectId}/leaderboards/{leaderboardId}/versions/{versionId}/tiers/{tierId}/scores";

            List<string> queryParams = new List<string>();


            var offsetStringValue = Offset.ToString();
            queryParams = AddParamsToQueryParams(queryParams, "offset", offsetStringValue);

            var limitStringValue = Limit.ToString();
            queryParams = AddParamsToQueryParams(queryParams, "limit", limitStringValue);
            if (includeMetadata == true)
            {
                queryParams = AddParamsToQueryParams(queryParams, "includeMetadata", includeMetadata.ToString());
            }
            if (queryParams.Count > 0)
            {
                PathAndQueryParams = $"{PathAndQueryParams}?{string.Join("&", queryParams)}";
            }
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="accessToken">The auth access token to use.</param>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken,
            Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accessToken.AccessToken))
            {
                headers.Add("authorization", "Bearer " + accessToken.AccessToken);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes =
            {
            };

            string[] accepts =
            {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "GET";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
    /// <summary>
    /// GetLeaderboardVersionsRequest
    /// Get Archived Leaderboard Versions
    /// </summary>
    [Preserve]
    internal class GetLeaderboardVersionsRequest : LeaderboardsApiBaseRequest
    {
        /// <summary>Accessor for projectId </summary>
        [Preserve]
        public string ProjectId { get; }
        /// <summary>Accessor for leaderboardId </summary>
        [Preserve]
        public string LeaderboardId { get; }
        [Preserve]
        public int? Limit { get; }
        string PathAndQueryParams;

        /// <summary>
        /// GetLeaderboardVersions Request Object.
        /// Get Archived Leaderboard Versions
        /// </summary>
        /// <param name="projectId">The project's [Project ID](https://docs.unity.com/ugs-overview/ManagingUnityProjects.html)</param>
        /// <param name="leaderboardId">ID of the leaderboard</param>
        [Preserve]
        public GetLeaderboardVersionsRequest(string projectId, string leaderboardId, int? limit)
        {
            ProjectId = projectId;
            LeaderboardId = leaderboardId;
            Limit = limit;

            PathAndQueryParams = $"/v1/projects/{projectId}/leaderboards/{leaderboardId}/versions";

            List<string> queryParams = new List<string>();

            if (Limit != null)
            {
                var limitStringValue = Limit.ToString();
                queryParams = AddParamsToQueryParams(queryParams, "limit", limitStringValue);
            }

            if (queryParams.Count > 0)
            {
                PathAndQueryParams = $"{PathAndQueryParams}?{string.Join("&", queryParams)}";
            }
        }

        /// <summary>
        /// Helper function for constructing URL from request base path and
        /// query params.
        /// </summary>
        /// <param name="requestBasePath"></param>
        /// <returns></returns>
        public string ConstructUrl(string requestBasePath)
        {
            return requestBasePath + PathAndQueryParams;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public byte[] ConstructBody()
        {
            return null;
        }

        /// <summary>
        /// Helper function for constructing the headers.
        /// </summary>
        /// <param name="accessToken">The auth access token to use.</param>
        /// <param name="operationConfiguration">The operation configuration to use.</param>
        /// <returns>A dictionary representing the request headers.</returns>
        public Dictionary<string, string> ConstructHeaders(IAccessToken accessToken,
            Configuration operationConfiguration = null)
        {
            var headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(accessToken.AccessToken))
            {
                headers.Add("authorization", "Bearer " + accessToken.AccessToken);
            }

            // Analytics headers
            headers.Add("Unity-Client-Version", Application.unityVersion);
            headers.Add("Unity-Client-Mode", Scheduler.EngineStateHelper.IsPlaying ? "play" : "edit");

            string[] contentTypes =
            {
            };

            string[] accepts =
            {
                "application/json",
                "application/problem+json"
            };

            var acceptHeader = GenerateAcceptHeader(accepts);
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                headers.Add("Accept", acceptHeader);
            }
            var httpMethod = "GET";
            var contentTypeHeader = GenerateContentTypeHeader(contentTypes);
            if (!string.IsNullOrEmpty(contentTypeHeader))
            {
                headers.Add("Content-Type", contentTypeHeader);
            }
            else if (httpMethod == "POST" || httpMethod == "PATCH")
            {
                headers.Add("Content-Type", "application/json");
            }


            // We also check if there are headers that are defined as part of
            // the request configuration.
            if (operationConfiguration != null && operationConfiguration.Headers != null)
            {
                foreach (var pair in operationConfiguration.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            return headers;
        }
    }
}
