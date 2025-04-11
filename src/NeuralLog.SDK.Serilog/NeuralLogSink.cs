using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;
using NeuralLog.SDK.Models;

namespace NeuralLog.SDK.Serilog
{
    /// <summary>
    /// A Serilog sink that writes log events to NeuralLog.
    /// </summary>
    public class NeuralLogSink : ILogEventSink
    {
        private readonly IAILogger _aiLogger;
        private readonly IFormatProvider? _formatProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuralLogSink"/> class.
        /// </summary>
        /// <param name="logName">The name of the log in NeuralLog.</param>
        /// <param name="formatProvider">The format provider to use.</param>
        public NeuralLogSink(string logName, IFormatProvider? formatProvider = null)
            : this(NeuralLog.GetLogger(logName), formatProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuralLogSink"/> class.
        /// </summary>
        /// <param name="aiLogger">The NeuralLog AI logger.</param>
        /// <param name="formatProvider">The format provider to use.</param>
        public NeuralLogSink(IAILogger aiLogger, IFormatProvider? formatProvider = null)
        {
            _aiLogger = aiLogger ?? throw new ArgumentNullException(nameof(aiLogger));
            _formatProvider = formatProvider;
        }

        /// <inheritdoc />
        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            // Convert log level
            var neuralLogLevel = ConvertLogLevel(logEvent.Level);

            // Format message
            var message = logEvent.RenderMessage(_formatProvider);

            // Extract structured data
            var data = ExtractStructuredData(logEvent);

            // Log to NeuralLog
            if (logEvent.Exception != null)
            {
                _aiLogger.Log(neuralLogLevel, message, logEvent.Exception, data);
            }
            else
            {
                _aiLogger.Log(neuralLogLevel, message, data);
            }
        }

        private static LogLevel ConvertLogLevel(LogEventLevel level)
        {
            return level switch
            {
                LogEventLevel.Verbose => LogLevel.Debug,
                LogEventLevel.Debug => LogLevel.Debug,
                LogEventLevel.Information => LogLevel.Info,
                LogEventLevel.Warning => LogLevel.Warning,
                LogEventLevel.Error => LogLevel.Error,
                LogEventLevel.Fatal => LogLevel.Fatal,
                _ => LogLevel.Info
            };
        }

        private static Dictionary<string, object> ExtractStructuredData(LogEvent logEvent)
        {
            var data = new Dictionary<string, object>();

            // Add standard properties
            data["timestamp"] = logEvent.Timestamp.ToString("o");
            
            if (logEvent.MessageTemplate.Text != null)
            {
                data["messageTemplate"] = logEvent.MessageTemplate.Text;
            }

            // Add properties
            foreach (var property in logEvent.Properties)
            {
                // Skip special properties
                if (property.Key.StartsWith("@") || property.Key.StartsWith("$"))
                {
                    continue;
                }

                data[property.Key] = ConvertPropertyValue(property.Value);
            }

            return data;
        }

        private static object ConvertPropertyValue(LogEventPropertyValue value)
        {
            return value switch
            {
                ScalarValue scalarValue => scalarValue.Value ?? string.Empty,
                SequenceValue sequenceValue => ConvertSequenceValue(sequenceValue),
                StructureValue structureValue => ConvertStructureValue(structureValue),
                DictionaryValue dictionaryValue => ConvertDictionaryValue(dictionaryValue),
                _ => value.ToString() ?? string.Empty
            };
        }

        private static object[] ConvertSequenceValue(SequenceValue sequenceValue)
        {
            var result = new object[sequenceValue.Elements.Count];
            for (var i = 0; i < sequenceValue.Elements.Count; i++)
            {
                result[i] = ConvertPropertyValue(sequenceValue.Elements[i]);
            }
            return result;
        }

        private static Dictionary<string, object> ConvertStructureValue(StructureValue structureValue)
        {
            var result = new Dictionary<string, object>();
            foreach (var property in structureValue.Properties)
            {
                result[property.Name] = ConvertPropertyValue(property.Value);
            }
            return result;
        }

        private static Dictionary<string, object> ConvertDictionaryValue(DictionaryValue dictionaryValue)
        {
            var result = new Dictionary<string, object>();
            foreach (var element in dictionaryValue.Elements)
            {
                var key = ConvertPropertyValue(element.Key).ToString() ?? string.Empty;
                var value = ConvertPropertyValue(element.Value);
                result[key] = value;
            }
            return result;
        }
    }
}
