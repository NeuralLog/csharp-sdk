using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NeuralLog.SDK.Http;
using NeuralLog.SDK.Models;

namespace NeuralLog.SDK
{
    /// <summary>
    /// Implementation of the AI logger.
    /// </summary>
    public class AILogger : IAILogger
    {
        private readonly NeuralLogConfig _config;
        private readonly IHttpClient _httpClient;
        private readonly Dictionary<string, object> _context = new Dictionary<string, object>();
        private readonly ConcurrentQueue<LogEntry> _batchQueue = new ConcurrentQueue<LogEntry>();
        private readonly Timer _batchTimer;
        private readonly SemaphoreSlim _batchSemaphore = new SemaphoreSlim(1, 1);
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AILogger"/> class.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="config">The NeuralLog configuration.</param>
        /// <param name="httpClient">The HTTP client to use.</param>
        public AILogger(string logName, NeuralLogConfig config, IHttpClient httpClient)
        {
            LogName = logName ?? throw new ArgumentNullException(nameof(logName));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (_config.AsyncEnabled && _config.BatchIntervalMs > 0)
            {
                _batchTimer = new Timer(SendBatchCallback, null, _config.BatchIntervalMs, _config.BatchIntervalMs);
            }
        }

        /// <inheritdoc/>
        public string LogName { get; }

        /// <inheritdoc/>
        public Task Debug(string message)
        {
            return Log(LogLevel.Debug, message);
        }

        /// <inheritdoc/>
        public Task Debug(string message, Dictionary<string, object> data)
        {
            return Log(LogLevel.Debug, message, data);
        }

        /// <inheritdoc/>
        public Task Debug(string message, object data)
        {
            return Log(LogLevel.Debug, message, ConvertObjectToData(data));
        }

        /// <inheritdoc/>
        public Task Info(string message)
        {
            return Log(LogLevel.Info, message);
        }

        /// <inheritdoc/>
        public Task Info(string message, Dictionary<string, object> data)
        {
            return Log(LogLevel.Info, message, data);
        }

        /// <inheritdoc/>
        public Task Info(string message, object data)
        {
            return Log(LogLevel.Info, message, ConvertObjectToData(data));
        }

        /// <inheritdoc/>
        public Task Warning(string message)
        {
            return Log(LogLevel.Warning, message);
        }

        /// <inheritdoc/>
        public Task Warning(string message, Dictionary<string, object> data)
        {
            return Log(LogLevel.Warning, message, data);
        }

        /// <inheritdoc/>
        public Task Warning(string message, object data)
        {
            return Log(LogLevel.Warning, message, ConvertObjectToData(data));
        }

        /// <inheritdoc/>
        public Task Error(string message)
        {
            return Log(LogLevel.Error, message);
        }

        /// <inheritdoc/>
        public Task Error(string message, Exception exception)
        {
            return Log(LogLevel.Error, message, exception);
        }

        /// <inheritdoc/>
        public Task Error(string message, Dictionary<string, object> data)
        {
            return Log(LogLevel.Error, message, data);
        }

        /// <inheritdoc/>
        public Task Error(string message, Exception exception, Dictionary<string, object> data)
        {
            return Log(LogLevel.Error, message, exception, data);
        }

        /// <inheritdoc/>
        public Task Error(string message, object data)
        {
            return Log(LogLevel.Error, message, ConvertObjectToData(data));
        }

        /// <inheritdoc/>
        public Task Error(string message, Exception exception, object data)
        {
            return Log(LogLevel.Error, message, exception, ConvertObjectToData(data));
        }

        /// <inheritdoc/>
        public Task Fatal(string message)
        {
            return Log(LogLevel.Fatal, message);
        }

        /// <inheritdoc/>
        public Task Fatal(string message, Exception exception)
        {
            return Log(LogLevel.Fatal, message, exception);
        }

        /// <inheritdoc/>
        public Task Fatal(string message, Dictionary<string, object> data)
        {
            return Log(LogLevel.Fatal, message, data);
        }

        /// <inheritdoc/>
        public Task Fatal(string message, Exception exception, Dictionary<string, object> data)
        {
            return Log(LogLevel.Fatal, message, exception, data);
        }

        /// <inheritdoc/>
        public Task Fatal(string message, object data)
        {
            return Log(LogLevel.Fatal, message, ConvertObjectToData(data));
        }

        /// <inheritdoc/>
        public Task Fatal(string message, Exception exception, object data)
        {
            return Log(LogLevel.Fatal, message, exception, ConvertObjectToData(data));
        }

        /// <inheritdoc/>
        public Task Log(LogLevel level, string message)
        {
            return Log(level, message, null, null);
        }

        /// <inheritdoc/>
        public Task Log(LogLevel level, string message, Dictionary<string, object> data)
        {
            return Log(level, message, null, data);
        }

        /// <inheritdoc/>
        public Task Log(LogLevel level, string message, Exception exception)
        {
            return Log(level, message, exception, null);
        }

        /// <inheritdoc/>
        public async Task Log(LogLevel level, string message, Exception exception, Dictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            var logEntry = new LogEntry
            {
                Level = level,
                Message = message,
                Exception = exception != null ? ExceptionInfo.FromException(exception) : null
            };

            if (data != null)
            {
                foreach (var item in data)
                {
                    logEntry.Data[item.Key] = item.Value;
                }
            }

            if (_context.Count > 0)
            {
                foreach (var item in _context)
                {
                    if (!logEntry.Data.ContainsKey(item.Key))
                    {
                        logEntry.Data[item.Key] = item.Value;
                    }
                }
            }

            if (_config.AsyncEnabled)
            {
                if (_config.BatchSize > 1)
                {
                    _batchQueue.Enqueue(logEntry);

                    if (_batchQueue.Count >= _config.BatchSize)
                    {
                        await SendBatchAsync();
                    }
                }
                else
                {
                    await SendLogEntryAsync(logEntry);
                }
            }
            else
            {
                await SendLogEntryAsync(logEntry);
            }
        }

        /// <inheritdoc/>
        public void SetContext(Dictionary<string, object> context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context.Clear();
            foreach (var item in context)
            {
                _context[item.Key] = item.Value;
            }
        }

        /// <inheritdoc/>
        public async Task Flush()
        {
            if (_config.AsyncEnabled && _config.BatchSize > 1 && _batchQueue.Count > 0)
            {
                await SendBatchAsync();
            }
        }

        private async void SendBatchCallback(object state)
        {
            if (_batchQueue.Count > 0)
            {
                await SendBatchAsync();
            }
        }

        private async Task SendBatchAsync()
        {
            if (_batchQueue.IsEmpty)
            {
                return;
            }

            await _batchSemaphore.WaitAsync();

            try
            {
                var logEntries = new List<LogEntry>();
                while (logEntries.Count < _config.BatchSize && _batchQueue.TryDequeue(out var logEntry))
                {
                    logEntries.Add(logEntry);
                }

                if (logEntries.Count > 0)
                {
                    await SendLogEntriesBatchAsync(logEntries);
                }
            }
            finally
            {
                _batchSemaphore.Release();
            }
        }

        private async Task SendLogEntriesBatchAsync(List<LogEntry> logEntries)
        {
            try
            {
                var url = $"{_config.ServerUrl}/logs/{LogName}/batch";
                if (!string.IsNullOrEmpty(_config.Namespace) && _config.Namespace != "default")
                {
                    url = $"{_config.ServerUrl}/{_config.Namespace}/logs/{LogName}/batch";
                }

                var json = JsonSerializer.Serialize(logEntries, _config.SerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                if (_config.DebugEnabled)
                {
                    Console.Error.WriteLine($"Error sending batch log entries: {ex.Message}");
                }
            }
        }

        private async Task SendLogEntryAsync(LogEntry logEntry)
        {
            try
            {
                var url = $"{_config.ServerUrl}/logs/{LogName}";
                if (!string.IsNullOrEmpty(_config.Namespace) && _config.Namespace != "default")
                {
                    url = $"{_config.ServerUrl}/{_config.Namespace}/logs/{LogName}";
                }

                var json = JsonSerializer.Serialize(logEntry, _config.SerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                if (_config.DebugEnabled)
                {
                    Console.Error.WriteLine($"Error sending log entry: {ex.Message}");
                }
            }
        }

        private Dictionary<string, object> ConvertObjectToData(object data)
        {
            if (data == null)
            {
                return null;
            }

            if (data is Dictionary<string, object> dict)
            {
                return dict;
            }

            try
            {
                var json = JsonSerializer.Serialize(data, _config.SerializerOptions);
                var result = JsonSerializer.Deserialize<Dictionary<string, object>>(json, _config.SerializerOptions);
                return result;
            }
            catch
            {
                return new Dictionary<string, object> { ["data"] = data };
            }
        }

        /// <summary>
        /// Disposes the logger and releases resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the logger and releases resources.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _batchTimer?.Dispose();
                    _batchSemaphore.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
