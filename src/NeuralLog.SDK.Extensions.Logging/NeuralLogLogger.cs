using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NeuralLog.SDK.Models;
using MsLogLevel = Microsoft.Extensions.Logging.LogLevel;
using NeuralLogLevel = NeuralLog.SDK.Models.LogLevel;

namespace NeuralLog.SDK.Extensions.Logging
{
    /// <summary>
    /// Implementation of <see cref="ILogger"/> that writes logs to NeuralLog.
    /// </summary>
    public class NeuralLogLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IAILogger _aiLogger;
        private readonly NeuralLogLoggerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuralLogLogger"/> class.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <param name="aiLogger">The NeuralLog AI logger.</param>
        /// <param name="options">The options for the logger.</param>
        public NeuralLogLogger(string categoryName, IAILogger aiLogger, NeuralLogLoggerOptions options)
        {
            _categoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
            _aiLogger = aiLogger ?? throw new ArgumentNullException(nameof(aiLogger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return NeuralLogLoggerScope.Push(state);
        }

        /// <inheritdoc />
        public bool IsEnabled(MsLogLevel logLevel)
        {
            return logLevel != MsLogLevel.None;
        }

        /// <inheritdoc />
        public void Log<TState>(
            MsLogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            string message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) && exception == null)
            {
                return;
            }

            // Convert log level
            var neuralLogLevel = ConvertLogLevel(logLevel);

            // Extract structured data
            var data = new Dictionary<string, object>
            {
                ["category"] = _categoryName,
                ["eventId"] = eventId.Id
            };

            if (!string.IsNullOrEmpty(eventId.Name))
            {
                data["eventName"] = eventId.Name;
            }

            // Add scoped data
            if (NeuralLogLoggerScope.Current != null)
            {
                foreach (var scopeItem in NeuralLogLoggerScope.Current)
                {
                    if (scopeItem is Dictionary<string, object> dict)
                    {
                        foreach (var item in dict)
                        {
                            data[item.Key] = item.Value;
                        }
                    }
                    else if (scopeItem is KeyValuePair<string, object> kvp)
                    {
                        data[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        data[$"scope_{data.Count}"] = scopeItem;
                    }
                }
            }

            // Add state data if it's an IEnumerable<KeyValuePair<string, object>>
            if (state is IEnumerable<KeyValuePair<string, object>> stateProperties)
            {
                foreach (var item in stateProperties)
                {
                    data[item.Key] = item.Value;
                }
            }

            // Log to NeuralLog
            if (exception != null)
            {
                _aiLogger.Log(neuralLogLevel, message, exception, data);
            }
            else
            {
                _aiLogger.Log(neuralLogLevel, message, data);
            }
        }

        private static NeuralLogLevel ConvertLogLevel(MsLogLevel logLevel)
        {
            return logLevel switch
            {
                MsLogLevel.Trace => NeuralLogLevel.Debug,
                MsLogLevel.Debug => NeuralLogLevel.Debug,
                MsLogLevel.Information => NeuralLogLevel.Info,
                MsLogLevel.Warning => NeuralLogLevel.Warning,
                MsLogLevel.Error => NeuralLogLevel.Error,
                MsLogLevel.Critical => NeuralLogLevel.Fatal,
                _ => NeuralLogLevel.Info
            };
        }
    }
}
