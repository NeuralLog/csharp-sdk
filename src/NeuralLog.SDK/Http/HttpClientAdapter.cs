using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NeuralLog.SDK.Models;

namespace NeuralLog.SDK.Http
{
    /// <summary>
    /// Adapter for System.Net.Http.HttpClient that implements IHttpClient.
    /// </summary>
    public class HttpClientAdapter : IHttpClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly bool _ownsHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientAdapter"/> class with a new HttpClient.
        /// </summary>
        /// <param name="config">The NeuralLog configuration.</param>
        public HttpClientAdapter(NeuralLogConfig config)
        {
            _httpClient = new HttpClient
            {
                Timeout = config.Timeout
            };

            if (!string.IsNullOrEmpty(config.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", config.ApiKey);
            }

            foreach (var header in config.Headers)
            {
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            _ownsHttpClient = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientAdapter"/> class with an existing HttpClient.
        /// </summary>
        /// <param name="httpClient">The HttpClient to use.</param>
        public HttpClientAdapter(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _ownsHttpClient = false;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return await _httpClient.SendAsync(request, cancellationToken);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_ownsHttpClient)
            {
                _httpClient.Dispose();
            }
        }
    }
}
