using System.Collections.Generic;

namespace Unity.Services.Leaderboards.Internal
{
    /// <summary>
    /// Represents a set of configuration settings
    /// </summary>
    internal class Configuration
    {
        /// <summary>The base service path which is overridable. Should be set to a valid URL.</summary>
        public string BasePath;
        /// <summary>The configuration request timeout.</summary>
        public int? RequestTimeout;
        /// <summary>Number of retries to attempt the operation.</summary>
        public int? NumberOfRetries;
        /// <summary>Headers for the operation.</summary>
        public IDictionary<string, string> Headers;


        /// <summary>
        /// Configuration constructor.
        /// </summary>
        /// <param name="basePath">The base service path which is overridable. Should be set to a valid URL.</param>
        /// <param name="requestTimeout">Request timeout for the configuration.</param>
        /// <param name="numRetries">Number of retries for the configuration.</param>
        /// <param name="headers">Headers for the configuration.</param>
        public Configuration(string basePath, int? requestTimeout, int? numRetries, IDictionary<string, string> headers)
        {
            BasePath = basePath;
            RequestTimeout = requestTimeout;
            NumberOfRetries = numRetries;
            if(headers == null)
            {
                Headers = headers;
            }
            else
            {
                Headers = new Dictionary<string, string>(headers);
            }
        }

        /// <summary>
        /// Helper function for merging two configurations. Configuration `a` is
        /// considered the base configuration if it is a valid object. Certain
        /// values will be overridden if they are set to null within this
        /// configuration by configuration `b` and the headers will be merged.
        /// </summary>
        /// <param name="a">Base configuration.</param>
        /// <param name="b">Current configuration.</param>
        /// <returns>A Configuration consisting of the combined `a` and `b` configurations.</returns>
        public static Configuration MergeConfigurations(Configuration a, Configuration b)
        {
            // Check if either inputs are `null`, if they are, we return
            // whichever is not `null`, if both are `null`, we return `b` which
            // will be `null`.
            if(a == null || b == null)
            {
                return a ?? b;
            }

            Configuration mergedConfig = new Configuration(a.BasePath, a.RequestTimeout, a.NumberOfRetries, a.Headers);

            if(mergedConfig.BasePath == null)
            {
                mergedConfig.BasePath = b.BasePath;
            }

            var headers = new Dictionary<string, string>();

            if (b.Headers != null)
            {
                foreach (var pair in b.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            if (mergedConfig.Headers != null)
            {
                foreach (var pair in mergedConfig.Headers)
                {
                    headers[pair.Key] = pair.Value;
                }
            }

            mergedConfig.Headers = headers;
            mergedConfig.RequestTimeout = mergedConfig.RequestTimeout ?? b.RequestTimeout;
            mergedConfig.NumberOfRetries = mergedConfig.NumberOfRetries ?? b.NumberOfRetries;


            return mergedConfig;
        }
    }
}
