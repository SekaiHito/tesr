using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;
using PlaywrightTests.Config;

namespace PlaywrightTests.Core.API
{
    /// <summary>
    /// Base class for API client with common HTTP methods and error handling.
    /// Provides centralized API interaction patterns with logging and retry logic.
    /// </summary>
    public abstract class BaseApiClient
    {
        protected readonly ILogger _logger;
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;

        protected BaseApiClient(ILogger logger, string baseUrl = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _baseUrl = baseUrl ?? ConfigManager.Environment.ApiUrl;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Sets authorization header.
        /// </summary>
        public void SetAuthorizationHeader(string token)
        {
            _logger.Information("Setting authorization header");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Makes GET request.
        /// </summary>
        public async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> headers = null) where T : class
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint}";
                _logger.Information("GET request to: {Url}", url);

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                AddHeaders(request, headers);

                var response = await _httpClient.SendAsync(request);
                return await HandleResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GET request failed for endpoint: {Endpoint}", endpoint);
                throw;
            }
        }

        /// <summary>
        /// Makes POST request.
        /// </summary>
        public async Task<T> PostAsync<T>(string endpoint, object data = null,
            Dictionary<string, string> headers = null) where T : class
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint}";
                _logger.Information("POST request to: {Url}", url);

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                AddHeaders(request, headers);

                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    _logger.Debug("POST data: {Data}", json);
                }

                var response = await _httpClient.SendAsync(request);
                return await HandleResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "POST request failed for endpoint: {Endpoint}", endpoint);
                throw;
            }
        }

        /// <summary>
        /// Makes PUT request.
        /// </summary>
        public async Task<T> PutAsync<T>(string endpoint, object data = null,
            Dictionary<string, string> headers = null) where T : class
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint}";
                _logger.Information("PUT request to: {Url}", url);

                var request = new HttpRequestMessage(HttpMethod.Put, url);
                AddHeaders(request, headers);

                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);
                return await HandleResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "PUT request failed for endpoint: {Endpoint}", endpoint);
                throw;
            }
        }

        /// <summary>
        /// Makes DELETE request.
        /// </summary>
        public async Task<T> DeleteAsync<T>(string endpoint, Dictionary<string, string> headers = null) where T : class
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint}";
                _logger.Information("DELETE request to: {Url}", url);

                var request = new HttpRequestMessage(HttpMethod.Delete, url);
                AddHeaders(request, headers);

                var response = await _httpClient.SendAsync(request);
                return await HandleResponseAsync<T>(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DELETE request failed for endpoint: {Endpoint}", endpoint);
                throw;
            }
        }

        /// <summary>
        /// Handles HTTP response with error checking.
        /// </summary>
        protected async Task<T> HandleResponseAsync<T>(HttpResponseMessage response) where T : class
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.Information("Response status: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.Error("API error - Status: {StatusCode}, Content: {Content}",
                        response.StatusCode, content);
                    throw new HttpRequestException(
                        $"API request failed with status {response.StatusCode}: {content}");
                }

                if (string.IsNullOrEmpty(content))
                    return null;

                return JsonSerializer.Deserialize<T>(content);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to handle response");
                throw;
            }
        }

        /// <summary>
        /// Adds headers to request.
        /// </summary>
        protected void AddHeaders(HttpRequestMessage request, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
        }

        /// <summary>
        /// Disposes HTTP client.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
