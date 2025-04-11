namespace NeuralLog.SDK.Models
{
    /// <summary>
    /// Represents the severity level of a log entry.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug level for detailed troubleshooting information.
        /// </summary>
        Debug,

        /// <summary>
        /// Info level for general information about system operation.
        /// </summary>
        Info,

        /// <summary>
        /// Warning level for potential issues that are not errors.
        /// </summary>
        Warning,

        /// <summary>
        /// Error level for runtime errors or unexpected conditions.
        /// </summary>
        Error,

        /// <summary>
        /// Fatal level for severe errors that cause the application to crash or terminate.
        /// </summary>
        Fatal
    }
}
