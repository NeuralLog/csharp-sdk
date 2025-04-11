using System;
using System.Collections.Generic;
using System.Threading;

namespace NeuralLog.SDK.Extensions.Logging
{
    /// <summary>
    /// Represents a scope for the NeuralLog logger.
    /// </summary>
    internal class NeuralLogLoggerScope : IDisposable
    {
        private static readonly AsyncLocal<NeuralLogLoggerScope?> _current = new AsyncLocal<NeuralLogLoggerScope?>();
        private readonly NeuralLogLoggerScope? _parent;
        private readonly object _state;
        private bool _disposed;

        /// <summary>
        /// Gets the current scope.
        /// </summary>
        public static IReadOnlyList<object>? Current
        {
            get
            {
                var scope = _current.Value;
                if (scope == null)
                {
                    return null;
                }

                var result = new List<object>();
                while (scope != null)
                {
                    result.Add(scope._state);
                    scope = scope._parent;
                }

                result.Reverse();
                return result;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuralLogLoggerScope"/> class.
        /// </summary>
        /// <param name="state">The state object.</param>
        /// <param name="parent">The parent scope.</param>
        private NeuralLogLoggerScope(object state, NeuralLogLoggerScope? parent)
        {
            _state = state;
            _parent = parent;
        }

        /// <summary>
        /// Pushes a new scope onto the stack.
        /// </summary>
        /// <typeparam name="TState">The type of the state object.</typeparam>
        /// <param name="state">The state object.</param>
        /// <returns>A disposable object that ends the scope when disposed.</returns>
        public static IDisposable Push<TState>(TState state)
        {
            var parent = _current.Value;
            var newScope = new NeuralLogLoggerScope(state!, parent);
            _current.Value = newScope;
            return newScope;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _current.Value = _parent;
            _disposed = true;
        }
    }
}
