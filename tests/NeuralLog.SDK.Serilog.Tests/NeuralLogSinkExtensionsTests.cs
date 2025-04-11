using System;
using Moq;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Xunit;

namespace NeuralLog.SDK.Serilog.Tests
{
    public class NeuralLogSinkExtensionsTests
    {
        [Fact]
        public void NeuralLog_WithLogName_DoesNotThrowException()
        {
            // Arrange
            var loggerConfiguration = new LoggerConfiguration();

            // Act & Assert
            var exception = Record.Exception(() => loggerConfiguration.WriteTo.NeuralLog("test-log"));
            Assert.Null(exception);
        }

        [Fact]
        public void NeuralLog_WithNullLoggerConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            LoggerSinkConfiguration nullConfig = null!;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => nullConfig.NeuralLog("test-log"));
            Assert.Equal("loggerConfiguration", exception.ParamName);
        }

        [Fact]
        public void NeuralLog_WithEmptyLogName_ThrowsArgumentException()
        {
            // Arrange
            var loggerConfiguration = new LoggerConfiguration();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => loggerConfiguration.WriteTo.NeuralLog(""));
            Assert.Equal("logName", exception.ParamName);
        }

        [Fact]
        public void NeuralLog_WithAILogger_DoesNotThrowException()
        {
            // Arrange
            var loggerConfiguration = new LoggerConfiguration();
            var mockAILogger = new Mock<IAILogger>();

            // Act & Assert
            var exception = Record.Exception(() => loggerConfiguration.WriteTo.NeuralLog(mockAILogger.Object));
            Assert.Null(exception);
        }

        [Fact]
        public void NeuralLog_WithNullAILogger_ThrowsArgumentNullException()
        {
            // Arrange
            var loggerConfiguration = new LoggerConfiguration();
            IAILogger nullLogger = null!;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => loggerConfiguration.WriteTo.NeuralLog(nullLogger));
            Assert.Equal("aiLogger", exception.ParamName);
        }
    }
}
