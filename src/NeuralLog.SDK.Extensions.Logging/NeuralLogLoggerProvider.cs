using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NeuralLog.SDK.Extensions.Logging
{
    /// <summary>
    /// Provider for the NeuralLog logger.
    /// </summary>
    [ProviderAlias("NeuralLog")]
    public class NeuralLogLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, NeuralLogLogger> _loggers = new ConcurrentDictionary<string, NeuralLogLogger>();
        private readonly IAILogger _aiLogger;
        private readonly NeuralLogLoggerOptions _options;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuralLogLoggerProvider"/> class.
        /// </summary>
        /// <param name="logName">The name of the log in NeuralLog.</param>
        /// <param name="options">The options for the logger.</param>
        public NeuralLogLoggerProvider(string logName, IOptions<NeuralLogLoggerOptions> options)
            : this(logName, options.Value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuralLogLoggerProvider"/> class.
        /// </summary>
        /// <param name="logName">The name of the log in NeuralLog.</param>
        /// <param name="options">The options for the logger.</param>
        public NeuralLogLoggerProvider(string logName, NeuralLogLoggerOptions options)
        {
            if (string.IsNullOrEmpty(logName))
            {
                throw new ArgumentException("Log name cannot be null or empty.", nameof(logName));
            }

            _options = options ?? throw new ArgumentNullException(nameof(options));
            _aiLogger = NeuralLog.GetLogger(logName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuralLogLoggerProvider"/> class.
        /// </summary>
        /// <param name="aiLogger">The NeuralLog AI logger.</param>
        /// <param name="options">The options for the logger.</param>
        public NeuralLogLoggerProvider(IAILogger aiLogger, NeuralLogLoggerOptions options)
        {
            _aiLogger = aiLogger ?? throw new ArgumentNullException(nameof(aiLogger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(NeuralLogLoggerProvider));
            }

            return _loggers.GetOrAdd(categoryName, name => new NeuralLogLogger(name, _aiLogger, _options));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _loggers.Clear();
        }
    }
}
