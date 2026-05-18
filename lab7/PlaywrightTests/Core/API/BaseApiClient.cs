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
    public abstract class BaseApiClient : IDisposable
    {
        protected readonly ILogger _logger;
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;

        protected BaseApiClient(ILogger logger, string? baseUrl = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            // Тимчасово замініть ConfigManager на пряме посилання:
            _baseUrl = "https://dummyjson.com"; 
            _httpClient = new HttpClient();
        }

        public void SetAuthorizationHeader(string token)
        {
            _logger.Information("Setting authorization header");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string>? headers = null) where T : class
        {
            var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
            _logger.Information("GET request to: {Url}", url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddHeaders(request, headers);

            var response = await _httpClient.SendAsync(request);
            return await HandleResponseAsync<T>(response);
        }

        public async Task<T> PostAsync<T>(string endpoint, object? data = null, Dictionary<string, string>? headers = null) where T : class
        {
            var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
            _logger.Information("POST request to: {Url}", url);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            AddHeaders(request, headers);

            if (data != null)
            {
                // Додаємо опції, щоб назви полів були camelCase (як того хоче API)
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(data, options);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            var response = await _httpClient.SendAsync(request);
            return await HandleResponseAsync<T>(response);
        }

        public async Task<T> PutAsync<T>(string endpoint, object? data = null, Dictionary<string, string>? headers = null) where T : class
        {
            var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
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

        public async Task<T> DeleteAsync<T>(string endpoint, Dictionary<string, string>? headers = null) where T : class
        {
            var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            AddHeaders(request, headers);
            var response = await _httpClient.SendAsync(request);
            return await HandleResponseAsync<T>(response);
        }

        protected async Task<T> HandleResponseAsync<T>(HttpResponseMessage response) where T : class
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.Information("Response status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API failed: {response.StatusCode} - {content}");
            }

            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        protected void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
        }

        public void Dispose() => _httpClient?.Dispose();
    }
}