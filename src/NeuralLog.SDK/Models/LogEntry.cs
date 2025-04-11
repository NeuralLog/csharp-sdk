using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NeuralLog.SDK.Models
{
    /// <summary>
    /// Represents a log entry to be sent to the NeuralLog server.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Gets or sets the unique identifier for the log entry.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the timestamp of the log entry.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets or sets the severity level of the log entry.
        /// </summary>
        [JsonPropertyName("level")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the structured data associated with the log entry.
        /// </summary>
        [JsonPropertyName("data")]
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the exception information if the log entry represents an error.
        /// </summary>
        [JsonPropertyName("exception")]
        public ExceptionInfo Exception { get; set; }
    }

    /// <summary>
    /// Represents information about an exception.
    /// </summary>
    public class ExceptionInfo
    {
        /// <summary>
        /// Gets or sets the type of the exception.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the exception message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace of the exception.
        /// </summary>
        [JsonPropertyName("stackTrace")]
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the inner exception information.
        /// </summary>
        [JsonPropertyName("innerException")]
        public ExceptionInfo InnerException { get; set; }

        /// <summary>
        /// Creates an ExceptionInfo from an Exception.
        /// </summary>
        /// <param name="exception">The exception to convert.</param>
        /// <returns>An ExceptionInfo object representing the exception.</returns>
        public static ExceptionInfo FromException(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            return new ExceptionInfo
            {
                Type = exception.GetType().FullName,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                InnerException = FromException(exception.InnerException)
            };
        }
    }
}
