using System;
using System.Collections.Generic;
using System.Text.Json;

namespace NeuralLog.SDK.Models
{
    /// <summary>
    /// Configuration options for the NeuralLog SDK.
    /// </summary>
    public class NeuralLogConfig
    {
        /// <summary>
        /// Gets or sets the URL of the NeuralLog server.
        /// </summary>
        public string ServerUrl { get; set; } = "http://localhost:3030";

        /// <summary>
        /// Gets or sets the namespace to use for logs.
        /// </summary>
        public string Namespace { get; set; } = "default";

        /// <summary>
        /// Gets or sets the API key for authentication.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable asynchronous logging.
        /// </summary>
        public bool AsyncEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the batch size for batched logging.
        /// </summary>
        public int BatchSize { get; set; } = 100;

        /// <summary>
        /// Gets or sets the batch interval in milliseconds.
        /// </summary>
        public int BatchIntervalMs { get; set; } = 5000;

        /// <summary>
        /// Gets or sets the maximum number of retries for failed requests.
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Gets or sets the retry backoff in milliseconds.
        /// </summary>
        public int RetryBackoffMs { get; set; } = 1000;

        /// <summary>
        /// Gets or sets a value indicating whether to enable debug logging.
        /// </summary>
        public bool DebugEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the timeout for HTTP requests.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets or sets the maximum number of concurrent connections.
        /// </summary>
        public int MaxConnections { get; set; } = 10;

        /// <summary>
        /// Gets or sets custom HTTP headers to include in requests.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the JSON serializer options.
        /// </summary>
        public JsonSerializerOptions SerializerOptions { get; set; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }
}
