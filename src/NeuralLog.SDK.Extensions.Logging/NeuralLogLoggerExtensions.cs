using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace NeuralLog.SDK.Extensions.Logging
{
    /// <summary>
    /// Extension methods for adding NeuralLog logger to the logging builder.
    /// </summary>
    public static class NeuralLogLoggerExtensions
    {
        /// <summary>
        /// Adds a NeuralLog logger to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the logger to.</param>
        /// <param name="logName">The name of the log in NeuralLog.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
        public static ILoggingBuilder AddNeuralLog(this ILoggingBuilder builder, string logName)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (string.IsNullOrEmpty(logName))
            {
                throw new ArgumentException("Log name cannot be null or empty.", nameof(logName));
            }

            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, NeuralLogLoggerProvider>(
                    serviceProvider =>
                    {
                        var options = new NeuralLogLoggerOptions();
                        return new NeuralLogLoggerProvider(logName, options);
                    }));

            return builder;
        }

        /// <summary>
        /// Adds a NeuralLog logger to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the logger to.</param>
        /// <param name="logName">The name of the log in NeuralLog.</param>
        /// <param name="configure">A delegate to configure the <see cref="NeuralLogLoggerOptions"/>.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
        public static ILoggingBuilder AddNeuralLog(
            this ILoggingBuilder builder,
            string logName,
            Action<NeuralLogLoggerOptions> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (string.IsNullOrEmpty(logName))
            {
                throw new ArgumentException("Log name cannot be null or empty.", nameof(logName));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, NeuralLogLoggerProvider>(
                    serviceProvider =>
                    {
                        var options = new NeuralLogLoggerOptions();
                        configure(options);
                        return new NeuralLogLoggerProvider(logName, options);
                    }));

            return builder;
        }

        /// <summary>
        /// Adds a NeuralLog logger to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the logger to.</param>
        /// <param name="aiLogger">The NeuralLog AI logger.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
        public static ILoggingBuilder AddNeuralLog(this ILoggingBuilder builder, AILogger aiLogger)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (aiLogger == null)
            {
                throw new ArgumentNullException(nameof(aiLogger));
            }

            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, NeuralLogLoggerProvider>(
                    serviceProvider =>
                    {
                        var options = new NeuralLogLoggerOptions();
                        return new NeuralLogLoggerProvider(aiLogger, options);
                    }));

            return builder;
        }

        /// <summary>
        /// Adds a NeuralLog logger to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the logger to.</param>
        /// <param name="aiLogger">The NeuralLog AI logger.</param>
        /// <param name="configure">A delegate to configure the <see cref="NeuralLogLoggerOptions"/>.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> so that additional calls can be chained.</returns>
        public static ILoggingBuilder AddNeuralLog(
            this ILoggingBuilder builder,
            AILogger aiLogger,
            Action<NeuralLogLoggerOptions> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (aiLogger == null)
            {
                throw new ArgumentNullException(nameof(aiLogger));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, NeuralLogLoggerProvider>(
                    serviceProvider =>
                    {
                        var options = new NeuralLogLoggerOptions();
                        configure(options);
                        return new NeuralLogLoggerProvider(aiLogger, options);
                    }));

            return builder;
        }
    }
}
