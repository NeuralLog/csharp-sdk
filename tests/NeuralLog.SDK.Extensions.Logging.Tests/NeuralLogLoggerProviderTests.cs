using System;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace NeuralLog.SDK.Extensions.Logging.Tests
{
    public class NeuralLogLoggerProviderTests
    {
        private readonly Mock<IAILogger> _mockAILogger;
        private readonly NeuralLogLoggerOptions _options;

        public NeuralLogLoggerProviderTests()
        {
            _mockAILogger = new Mock<IAILogger>();
            _options = new NeuralLogLoggerOptions();
        }

        [Fact]
        public void CreateLogger_ReturnsNeuralLogLogger()
        {
            // Arrange
            var provider = new NeuralLogLoggerProvider(_mockAILogger.Object, _options);

            // Act
            var logger = provider.CreateLogger("TestCategory");

            // Assert
            Assert.NotNull(logger);
            Assert.IsType<NeuralLogLogger>(logger);
        }

        [Fact]
        public void CreateLogger_WithSameCategoryName_ReturnsSameLoggerInstance()
        {
            // Arrange
            var provider = new NeuralLogLoggerProvider(_mockAILogger.Object, _options);

            // Act
            var logger1 = provider.CreateLogger("TestCategory");
            var logger2 = provider.CreateLogger("TestCategory");

            // Assert
            Assert.Same(logger1, logger2);
        }

        [Fact]
        public void CreateLogger_WithDifferentCategoryNames_ReturnsDifferentLoggerInstances()
        {
            // Arrange
            var provider = new NeuralLogLoggerProvider(_mockAILogger.Object, _options);

            // Act
            var logger1 = provider.CreateLogger("TestCategory1");
            var logger2 = provider.CreateLogger("TestCategory2");

            // Assert
            Assert.NotSame(logger1, logger2);
        }

        [Fact]
        public void Constructor_WithNullAILogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            IAILogger nullLogger = null!;
            var exception = Assert.Throws<ArgumentNullException>(() => new NeuralLogLoggerProvider(nullLogger, _options));
            Assert.Equal("aiLogger", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new NeuralLogLoggerProvider(_mockAILogger.Object, null!));
            Assert.Equal("options", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithEmptyLogName_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new NeuralLogLoggerProvider("", _options));
            Assert.Equal("logName", exception.ParamName);
        }

        [Fact]
        public void Dispose_ClearsLoggers()
        {
            // Arrange
            var provider = new NeuralLogLoggerProvider(_mockAILogger.Object, _options);
            var logger = provider.CreateLogger("TestCategory");

            // Act
            provider.Dispose();

            // Assert
            var exception = Assert.Throws<ObjectDisposedException>(() => provider.CreateLogger("AnotherCategory"));
            Assert.Equal("NeuralLogLoggerProvider", exception.ObjectName);
        }
    }
}
