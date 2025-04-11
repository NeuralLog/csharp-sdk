using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NeuralLog.SDK.Http
{
    /// <summary>
    /// Interface for HTTP client operations.
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// Sends an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response message.</returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    }
}
