# NeuralLog C# SDK

The C# SDK for NeuralLog provides a client library for interacting with the NeuralLog server from C# applications. It offers a familiar logging API and adapters for popular .NET logging frameworks.

## Requirements

- .NET 9.0 or later

## Installation

```bash
# Install via NuGet Package Manager
Install-Package NeuralLog.SDK

# Or using .NET CLI
dotnet add package NeuralLog.SDK
```

## Basic Usage

```csharp
using NeuralLog.SDK;
using System.Collections.Generic;

// Configure the SDK
var config = new NeuralLogConfig
{
    ServerUrl = "http://localhost:3030",
    Namespace = "default",
    AsyncEnabled = true,
    BatchSize = 100,
    BatchIntervalMs = 5000
};
NeuralLog.Configure(config);

// Get a logger
var logger = NeuralLog.GetLogger("my-application");

// Log a simple message
logger.Info("Hello, world!");

// Log with structured data
var data = new Dictionary<string, object>
{
    ["user"] = "john.doe",
    ["action"] = "login",
    ["ip"] = "192.168.1.1"
};
logger.Info("User logged in", data);

// Log an error with exception
try
{
    // Some code that might throw an exception
    throw new System.Exception("Something went wrong");
}
catch (System.Exception ex)
{
    logger.Error("Failed to process request", ex);
}
```

## Configuration Options

The C# SDK supports various configuration options:

```csharp
var config = new NeuralLogConfig
{
    // Required settings
    ServerUrl = "https://logs.example.com",
    Namespace = "production",

    // Optional settings
    ApiKey = "your-api-key",
    BatchSize = 100,
    BatchIntervalMs = 5000,
    MaxRetries = 3,
    RetryBackoffMs = 1000,
    AsyncEnabled = true,
    DebugEnabled = false,

    // HTTP client settings
    Timeout = TimeSpan.FromSeconds(30),
    MaxConnections = 10,

    // Custom HTTP headers
    Headers = new Dictionary<string, string>
    {
        ["X-Custom-Header"] = "value"
    }
};

NeuralLog.Configure(config);
```

## Advanced Features

### Context Data

```csharp
// Set global context data for all loggers
NeuralLog.SetGlobalContext(new Dictionary<string, object>
{
    ["application"] = "my-application",
    ["environment"] = "production",
    ["version"] = "1.0.0"
});

// Set logger-specific context data
logger.SetContext(new Dictionary<string, object>
{
    ["service"] = "user-service",
    ["instance"] = "user-service-1"
});
```

### Batching

The SDK supports batching of log messages to improve performance:

```csharp
var config = new NeuralLogConfig
{
    ServerUrl = "http://localhost:3030",
    Namespace = "default",
    AsyncEnabled = true,
    BatchSize = 100,
    BatchIntervalMs = 5000
};
NeuralLog.Configure(config);
```

With batching enabled, log messages are queued and sent in batches when:
- The batch size is reached
- The batch interval elapses
- The `Flush` method is called

### Flushing

To ensure all pending log messages are sent, you can flush the logger:

```csharp
// Flush a specific logger
await logger.Flush();

// Flush all loggers
await NeuralLog.FlushAll();
```

## Framework Integration

The NeuralLog C# SDK can be integrated with popular .NET logging frameworks. Framework-specific adapters will be added in future releases.

## Building from Source

```bash
# Clone the repository
git clone https://github.com/NeuralLog/csharp.git
cd csharp

# Build the solution
dotnet build

# Run the tests
dotnet test
```

## Documentation

Detailed documentation is available in the [docs](./docs) directory:

- [API Reference](./docs/api.md)
- [Configuration](./docs/configuration.md)
- [Architecture](./docs/architecture.md)
- [Examples](./docs/examples)

For integration guides and tutorials, visit the [NeuralLog Documentation Site](https://neurallog.github.io/docs/).

## Contributing

Contributions are welcome! Please read our [Contributing Guide](./CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## Related NeuralLog Components

- [NeuralLog Auth](https://github.com/NeuralLog/auth) - Authentication and authorization
- [NeuralLog Server](https://github.com/NeuralLog/server) - Core server functionality
- [NeuralLog Web](https://github.com/NeuralLog/web) - Web interface components
- [NeuralLog TypeScript Client SDK](https://github.com/NeuralLog/typescript-client-sdk) - TypeScript client SDK
- [NeuralLog Java Client SDK](https://github.com/NeuralLog/Java-client-sdk) - Java client SDK
- [NeuralLog Python SDK](https://github.com/NeuralLog/python) - Python SDK

## License

MIT
