using System;
using Serilog;
using Serilog.Configuration;

namespace NeuralLog.SDK.Serilog
{
    /// <summary>
    /// Extension methods for adding NeuralLog sink to Serilog.
    /// </summary>
    public static class NeuralLogSinkExtensions
    {
        /// <summary>
        /// Writes log events to NeuralLog.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="logName">The name of the log in NeuralLog.</param>
        /// <param name="formatProvider">The format provider to use.</param>
        /// <returns>The logger configuration.</returns>
        public static LoggerConfiguration NeuralLog(
            this LoggerSinkConfiguration loggerConfiguration,
            string logName,
            IFormatProvider? formatProvider = null)
        {
            if (loggerConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerConfiguration));
            }

            if (string.IsNullOrEmpty(logName))
            {
                throw new ArgumentException("Log name cannot be null or empty.", nameof(logName));
            }

            return loggerConfiguration.Sink(new NeuralLogSink(logName, formatProvider));
        }

        /// <summary>
        /// Writes log events to NeuralLog.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="aiLogger">The NeuralLog AI logger.</param>
        /// <param name="formatProvider">The format provider to use.</param>
        /// <returns>The logger configuration.</returns>
        public static LoggerConfiguration NeuralLog(
            this LoggerSinkConfiguration loggerConfiguration,
            IAILogger aiLogger,
            IFormatProvider? formatProvider = null)
        {
            if (loggerConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerConfiguration));
            }

            if (aiLogger == null)
            {
                throw new ArgumentNullException(nameof(aiLogger));
            }

            return loggerConfiguration.Sink(new NeuralLogSink(aiLogger, formatProvider));
        }
    }
}
