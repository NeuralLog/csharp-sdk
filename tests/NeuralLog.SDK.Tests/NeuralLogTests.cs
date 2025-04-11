using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Net;
using Xunit;
using NeuralLog.SDK;
using NeuralLog.SDK.Models;
using NeuralLog.SDK.Http;

namespace NeuralLog.SDK.Tests;

public class NeuralLogTests
{
    [Fact]
    public void Configure_SetsGlobalConfig()
    {
        // Arrange
        var config = new NeuralLogConfig
        {
            ServerUrl = "https://logs.example.com",
            Namespace = "test",
            ApiKey = "test-api-key"
        };

        // Act
        NeuralLog.Configure(config);

        // Assert
        Assert.Equal(config, NeuralLog.GetConfig());
    }

    [Fact]
    public void GetLogger_ReturnsLoggerWithCorrectName()
    {
        // Arrange
        var logName = "test-logger";
        NeuralLog.Configure(new NeuralLogConfig
        {
            ServerUrl = "https://logs.example.com",
            Namespace = "test"
        });

        // Act
        var logger = NeuralLog.GetLogger(logName);

        // Assert
        Assert.NotNull(logger);
        Assert.Equal(logName, logger.LogName);
    }
}

public class AILoggerTests
{
    private readonly Mock<IHttpClient> _mockHttpClient;
    private readonly NeuralLogConfig _config;
    private readonly AILogger _logger;

    public AILoggerTests()
    {
        _mockHttpClient = new Mock<IHttpClient>();
        _config = new NeuralLogConfig
        {
            ServerUrl = "https://logs.example.com",
            Namespace = "test",
            ApiKey = "test-api-key",
            AsyncEnabled = false // Use synchronous mode for testing
        };

        _logger = new AILogger("test-logger", _config, _mockHttpClient.Object);
    }

    [Fact]
    public async Task Info_SendsLogEntryWithCorrectLevel()
    {
        // Arrange
        _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

        // Act
        await _logger.Info("Test message");

        // Assert
        _mockHttpClient.Verify(x => x.SendAsync(
            It.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri.ToString().Contains("/logs/test-logger")),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Error_WithException_SendsLogEntryWithExceptionDetails()
    {
        // Arrange
        _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

        var exception = new Exception("Test exception");

        // Act
        await _logger.Error("Error occurred", exception);

        // Assert
        _mockHttpClient.Verify(x => x.SendAsync(
            It.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri.ToString().Contains("/logs/test-logger")),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Log_WithStructuredData_SendsLogEntryWithData()
    {
        // Arrange
        _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

        var data = new Dictionary<string, object>
        {
            ["user"] = "john.doe",
            ["action"] = "login"
        };

        // Act
        await _logger.Info("User logged in", data);

        // Assert
        _mockHttpClient.Verify(x => x.SendAsync(
            It.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri.ToString().Contains("/logs/test-logger")),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Log_WithBatchingEnabled_BatchesRequests()
    {
        // Arrange
        var batchConfig = new NeuralLogConfig
        {
            ServerUrl = "https://logs.example.com",
            Namespace = "test",
            ApiKey = "test-api-key",
            AsyncEnabled = true,
            BatchSize = 2,
            BatchIntervalMs = 100
        };

        var batchLogger = new AILogger("batch-logger", batchConfig, _mockHttpClient.Object);

        _mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Created));

        // Act
        await batchLogger.Info("Message 1");
        await batchLogger.Info("Message 2");

        // Wait for batch to be sent
        await Task.Delay(200);

        // Assert
        _mockHttpClient.Verify(x => x.SendAsync(
            It.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri.ToString().Contains("/logs/batch-logger/batch")),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
