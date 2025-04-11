using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeuralLog.SDK.Models;

namespace NeuralLog.SDK
{
    /// <summary>
    /// Interface for the AI logger.
    /// </summary>
    public interface IAILogger
    {
        /// <summary>
        /// Gets the name of the log.
        /// </summary>
        string LogName { get; }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Debug(string message);

        /// <summary>
        /// Logs a debug message with structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="data">The structured data to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Debug(string message, Dictionary<string, object> data);

        /// <summary>
        /// Logs a debug message with an object as structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="data">The object to log as structured data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Debug(string message, object data);

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Info(string message);

        /// <summary>
        /// Logs an info message with structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="data">The structured data to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Info(string message, Dictionary<string, object> data);

        /// <summary>
        /// Logs an info message with an object as structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="data">The object to log as structured data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Info(string message, object data);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Warning(string message);

        /// <summary>
        /// Logs a warning message with structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="data">The structured data to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Warning(string message, Dictionary<string, object> data);

        /// <summary>
        /// Logs a warning message with an object as structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="data">The object to log as structured data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Warning(string message, object data);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Error(string message);

        /// <summary>
        /// Logs an error message with an exception.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The exception to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Error(string message, Exception exception);

        /// <summary>
        /// Logs an error message with structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="data">The structured data to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Error(string message, Dictionary<string, object> data);

        /// <summary>
        /// Logs an error message with an exception and structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="data">The structured data to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Error(string message, Exception exception, Dictionary<string, object> data);

        /// <summary>
        /// Logs an error message with an object as structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="data">The object to log as structured data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Error(string message, object data);

        /// <summary>
        /// Logs an error message with an exception and an object as structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="data">The object to log as structured data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Error(string message, Exception exception, object data);

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Fatal(string message);

        /// <summary>
        /// Logs a fatal message with an exception.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The exception to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Fatal(string message, Exception exception);

        /// <summary>
        /// Logs a fatal message with structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="data">The structured data to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Fatal(string message, Dictionary<string, object> data);

        /// <summary>
        /// Logs a fatal message with an exception and structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="data">The structured data to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Fatal(string message, Exception exception, Dictionary<string, object> data);

        /// <summary>
        /// Logs a fatal message with an object as structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="data">The object to log as structured data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Fatal(string message, object data);

        /// <summary>
        /// Logs a fatal message with an exception and an object as structured data.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="data">The object to log as structured data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Fatal(string message, Exception exception, object data);

        /// <summary>
        /// Logs a message with the specified level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Log(LogLevel level, string message);

        /// <summary>
        /// Logs a message with the specified level and structured data.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="data">The structured data to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Log(LogLevel level, string message, Dictionary<string, object> data);

        /// <summary>
        /// Logs a message with the specified level and an exception.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The exception to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Log(LogLevel level, string message, Exception exception);

        /// <summary>
        /// Logs a message with the specified level, an exception, and structured data.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="data">The structured data to log.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Log(LogLevel level, string message, Exception exception, Dictionary<string, object> data);

        /// <summary>
        /// Sets context data for the logger.
        /// </summary>
        /// <param name="context">The context data to set.</param>
        void SetContext(Dictionary<string, object> context);

        /// <summary>
        /// Flushes any pending log entries.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Flush();
    }
}
