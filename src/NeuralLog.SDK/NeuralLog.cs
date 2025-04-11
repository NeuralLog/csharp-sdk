using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeuralLog.SDK.Http;
using NeuralLog.SDK.Models;

namespace NeuralLog.SDK
{
    /// <summary>
    /// Main entry point for the NeuralLog SDK.
    /// </summary>
    public static class NeuralLog
    {
        private static NeuralLogConfig _config = new NeuralLogConfig();
        private static readonly Dictionary<string, IAILogger> _loggers = new Dictionary<string, IAILogger>();
        private static readonly Dictionary<string, object> _globalContext = new Dictionary<string, object>();

        /// <summary>
        /// Configures the NeuralLog SDK.
        /// </summary>
        /// <param name="config">The configuration to use.</param>
        public static void Configure(NeuralLogConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        /// <returns>The current configuration.</returns>
        public static NeuralLogConfig GetConfig()
        {
            return _config;
        }

        /// <summary>
        /// Gets a logger with the specified name.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <returns>An AI logger.</returns>
        public static IAILogger GetLogger(string logName)
        {
            if (string.IsNullOrEmpty(logName))
            {
                throw new ArgumentNullException(nameof(logName));
            }

            lock (_loggers)
            {
                if (!_loggers.TryGetValue(logName, out var logger))
                {
                    var httpClient = new HttpClientAdapter(_config);
                    logger = new AILogger(logName, _config, httpClient);

                    if (_globalContext.Count > 0)
                    {
                        var context = new Dictionary<string, object>(_globalContext);
                        logger.SetContext(context);
                    }

                    _loggers[logName] = logger;
                }

                return logger;
            }
        }

        /// <summary>
        /// Sets global context data for all loggers.
        /// </summary>
        /// <param name="context">The context data to set.</param>
        public static void SetGlobalContext(Dictionary<string, object> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _globalContext.Clear();
            foreach (var item in context)
            {
                _globalContext[item.Key] = item.Value;
            }

            lock (_loggers)
            {
                foreach (var logger in _loggers.Values)
                {
                    var loggerContext = new Dictionary<string, object>(_globalContext);
                    logger.SetContext(loggerContext);
                }
            }
        }

        /// <summary>
        /// Flushes all loggers.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task FlushAll()
        {
            var tasks = new List<Task>();

            lock (_loggers)
            {
                foreach (var logger in _loggers.Values)
                {
                    tasks.Add(logger.Flush());
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}
