using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NeuralLog.SDK.Models;
using Xunit;
using MsLogLevel = Microsoft.Extensions.Logging.LogLevel;
using NeuralLogLevel = NeuralLog.SDK.Models.LogLevel;

namespace NeuralLog.SDK.Extensions.Logging.Tests
{
    public class NeuralLogLoggerTests
    {
        private readonly Mock<IAILogger> _mockAILogger;
        private readonly NeuralLogLoggerOptions _options;
        private readonly NeuralLogLogger _logger;

        public NeuralLogLoggerTests()
        {
            _mockAILogger = new Mock<IAILogger>();
            _options = new NeuralLogLoggerOptions();
            _logger = new NeuralLogLogger("TestCategory", _mockAILogger.Object, _options);
        }

        [Fact]
        public void Log_WithInfoLevel_CallsAILoggerWithInfoLevel()
        {
            // Arrange
            var logLevel = MsLogLevel.Information;
            var eventId = new EventId(1, "TestEvent");
            var state = "Test message";
            Exception? exception = null;
            Func<string, Exception?, string> formatter = (s, e) => s;

            // Act
            _logger.Log(logLevel, eventId, state, exception, formatter);

            // Assert
            _mockAILogger.Verify(
                x => x.Log(
                    It.Is<NeuralLogLevel>(l => l == NeuralLogLevel.Info),
                    It.Is<string>(m => m == "Test message"),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);
        }

        [Fact]
        public void Log_WithErrorAndException_CallsAILoggerWithErrorLevelAndException()
        {
            // Arrange
            var logLevel = MsLogLevel.Error;
            var eventId = new EventId(2, "ErrorEvent");
            var state = "Error message";
            var exception = new InvalidOperationException("Test exception");
            Func<string, Exception?, string> formatter = (s, e) => s;

            // Act
            _logger.Log(logLevel, eventId, state, exception, formatter);

            // Assert
            _mockAILogger.Verify(
                x => x.Log(
                    It.Is<NeuralLogLevel>(l => l == NeuralLogLevel.Error),
                    It.Is<string>(m => m == "Error message"),
                    It.Is<Exception?>(e => e == exception),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);
        }

        [Fact]
        public void Log_WithScope_IncludesScopeDataInLogEntry()
        {
            // Arrange
            var logLevel = MsLogLevel.Information;
            var eventId = new EventId(3, "ScopedEvent");
            var state = "Scoped message";
            Exception? exception = null;
            Func<string, Exception?, string> formatter = (s, e) => s;

            Dictionary<string, object>? capturedData = null;
            _mockAILogger
                .Setup(x => x.Log(It.IsAny<NeuralLogLevel>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Callback<NeuralLogLevel, string, Dictionary<string, object>>((_, _, data) => capturedData = data);

            // Act
            using (var scope = _logger.BeginScope(new Dictionary<string, object> { ["ScopeKey"] = "ScopeValue" }))
            {
                _logger.Log(logLevel, eventId, state, exception, formatter);
            }

            // Assert
            Assert.NotNull(capturedData);
            Assert.Contains("ScopeKey", capturedData.Keys);
            Assert.Equal("ScopeValue", capturedData["ScopeKey"]);
        }

        [Fact]
        public void Log_WithStructuredState_IncludesStateDataInLogEntry()
        {
            // Arrange
            var logLevel = MsLogLevel.Information;
            var eventId = new EventId(4, "StructuredEvent");
            var state = new[] { new KeyValuePair<string, object>("StateKey", "StateValue") };
            Exception? exception = null;
            Func<IEnumerable<KeyValuePair<string, object>>, Exception?, string> formatter = (s, e) => "Structured message";

            Dictionary<string, object>? capturedData = null;
            _mockAILogger
                .Setup(x => x.Log(It.IsAny<NeuralLogLevel>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Callback<NeuralLogLevel, string, Dictionary<string, object>>((_, _, data) => capturedData = data);

            // Act
            _logger.Log(logLevel, eventId, state, exception, formatter);

            // Assert
            Assert.NotNull(capturedData);
            Assert.Contains("StateKey", capturedData.Keys);
            Assert.Equal("StateValue", capturedData["StateKey"]);
        }

        [Theory]
        [InlineData(MsLogLevel.Trace, NeuralLogLevel.Debug)]
        [InlineData(MsLogLevel.Debug, NeuralLogLevel.Debug)]
        [InlineData(MsLogLevel.Information, NeuralLogLevel.Info)]
        [InlineData(MsLogLevel.Warning, NeuralLogLevel.Warning)]
        [InlineData(MsLogLevel.Error, NeuralLogLevel.Error)]
        [InlineData(MsLogLevel.Critical, NeuralLogLevel.Fatal)]
        public void Log_WithDifferentLogLevels_MapsToCorrectNeuralLogLevel(
            MsLogLevel inputLevel,
            NeuralLogLevel expectedLevel)
        {
            // Arrange
            var eventId = new EventId(5, "LevelTestEvent");
            var state = "Level test message";
            Exception? exception = null;
            Func<string, Exception?, string> formatter = (s, e) => s;

            NeuralLogLevel? capturedLevel = null;
            _mockAILogger
                .Setup(x => x.Log(It.IsAny<NeuralLogLevel>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Callback<NeuralLogLevel, string, Dictionary<string, object>>((level, _, _) => capturedLevel = level);

            // Act
            _logger.Log(inputLevel, eventId, state, exception, formatter);

            // Assert
            Assert.Equal(expectedLevel, capturedLevel);
        }
    }
}
