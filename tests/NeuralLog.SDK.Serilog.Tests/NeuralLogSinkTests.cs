using System;
using System.Collections.Generic;
using Moq;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;
using NeuralLog.SDK.Models;

namespace NeuralLog.SDK.Serilog.Tests
{
    public class NeuralLogSinkTests
    {
        private readonly Mock<IAILogger> _mockAILogger;
        private readonly NeuralLogSink _sink;

        public NeuralLogSinkTests()
        {
            _mockAILogger = new Mock<IAILogger>();
            _sink = new NeuralLogSink(_mockAILogger.Object);
        }

        [Fact]
        public void Emit_WithInfoLevel_CallsAILoggerWithInfoLevel()
        {
            // Arrange
            var logEvent = CreateLogEvent(LogEventLevel.Information, "Test message");

            // Act
            _sink.Emit(logEvent);

            // Assert
            _mockAILogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Info),
                    It.Is<string>(m => m == "Test message"),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);
        }

        [Fact]
        public void Emit_WithErrorAndException_CallsAILoggerWithErrorLevelAndException()
        {
            // Arrange
            var exception = new InvalidOperationException("Test exception");
            var logEvent = CreateLogEvent(LogEventLevel.Error, "Error message", exception);

            // Act
            _sink.Emit(logEvent);

            // Assert
            _mockAILogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.Is<string>(m => m == "Error message"),
                    It.Is<Exception>(e => e == exception),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);
        }

        [Fact]
        public void Emit_WithStructuredData_IncludesDataInLogEntry()
        {
            // Arrange
            var properties = new List<LogEventProperty>
            {
                new LogEventProperty("User", new ScalarValue("john.doe")),
                new LogEventProperty("Action", new ScalarValue("login"))
            };
            var logEvent = CreateLogEvent(LogEventLevel.Information, "User logged in", properties: properties);

            Dictionary<string, object>? capturedData = null;
            _mockAILogger
                .Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Callback<LogLevel, string, Dictionary<string, object>>((_, _, data) => capturedData = data);

            // Act
            _sink.Emit(logEvent);

            // Assert
            Assert.NotNull(capturedData);
            Assert.Contains("User", capturedData.Keys);
            Assert.Equal("john.doe", capturedData["User"]);
            Assert.Contains("Action", capturedData.Keys);
            Assert.Equal("login", capturedData["Action"]);
        }

        [Theory]
        [InlineData(LogEventLevel.Verbose, LogLevel.Debug)]
        [InlineData(LogEventLevel.Debug, LogLevel.Debug)]
        [InlineData(LogEventLevel.Information, LogLevel.Info)]
        [InlineData(LogEventLevel.Warning, LogLevel.Warning)]
        [InlineData(LogEventLevel.Error, LogLevel.Error)]
        [InlineData(LogEventLevel.Fatal, LogLevel.Fatal)]
        public void Emit_WithDifferentLogLevels_MapsToCorrectNeuralLogLevel(
            LogEventLevel inputLevel,
            LogLevel expectedLevel)
        {
            // Arrange
            var logEvent = CreateLogEvent(inputLevel, "Level test message");

            LogLevel? capturedLevel = null;
            _mockAILogger
                .Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Callback<LogLevel, string, Dictionary<string, object>>((level, _, _) => capturedLevel = level);

            // Act
            _sink.Emit(logEvent);

            // Assert
            Assert.Equal(expectedLevel, capturedLevel);
        }

        private static LogEvent CreateLogEvent(
            LogEventLevel level,
            string messageText,
            Exception? exception = null,
            IEnumerable<LogEventProperty>? properties = null)
        {
            var messageTemplate = new MessageTemplate(new[] { new TextToken(messageText) });
            var logEventProperties = new List<LogEventProperty>();

            if (properties != null)
            {
                logEventProperties.AddRange(properties);
            }

            return new LogEvent(
                DateTimeOffset.Now,
                level,
                exception,
                messageTemplate,
                logEventProperties);
        }
    }
}
