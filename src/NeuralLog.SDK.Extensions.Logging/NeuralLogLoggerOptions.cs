namespace NeuralLog.SDK.Extensions.Logging
{
    /// <summary>
    /// Options for the NeuralLog logger.
    /// </summary>
    public class NeuralLogLoggerOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to include scopes in the log data.
        /// </summary>
        public bool IncludeScopes { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include event IDs in the log data.
        /// </summary>
        public bool IncludeEventIds { get; set; } = true;
    }
}
