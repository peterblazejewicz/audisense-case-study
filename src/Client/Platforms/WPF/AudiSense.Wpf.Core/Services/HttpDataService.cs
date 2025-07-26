using System.Net.Http;
using System.Text;
using System.Text.Json;

using AudiSense.Client.Shared.Services.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AudiSense.Wpf.Core.Services;

/// <summary>
/// HTTP-based implementation of data service
/// </summary>
public class HttpDataService : IDataService
{
    private readonly ILogger<HttpDataService> _logger;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public HttpDataService(ILogger<HttpDataService> logger, HttpClient httpClient, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;

        // Get base URL from configuration, fallback to default API port
        var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:51704";

        // Ensure the base URL doesn't end with /api if it's already there
        if (baseUrl.EndsWith("/api"))
        {
            baseUrl = baseUrl.Substring(0, baseUrl.Length - 4);
        }

        _httpClient.BaseAddress = new Uri(baseUrl);
        _logger.LogInformation("Using API base URL: {BaseUrl}", baseUrl);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint) where T : class
    {
        _logger.LogInformation("Fetching data from endpoint: {Endpoint}", endpoint);

        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, _jsonOptions);
            }
            _logger.LogWarning("Failed to fetch data from {Endpoint}. Status: {StatusCode}", endpoint, response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching data from {Endpoint}", endpoint);
        }

        return default(T);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        where TRequest : class
        where TResponse : class
    {
        _logger.LogInformation("Posting data to endpoint: {Endpoint}", endpoint);

        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
            }
            _logger.LogWarning("Failed to post data to {Endpoint}. Status: {StatusCode}", endpoint, response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while posting data to {Endpoint}", endpoint);
        }

        return default(TResponse);
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        _logger.LogInformation("Deleting resource at endpoint: {Endpoint}", endpoint);

        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting resource at {Endpoint}", endpoint);
            return false;
        }
    }

    public async Task<bool> UpdateAsync<T>(string endpoint, T data) where T : class
    {
        _logger.LogInformation("Updating resource at endpoint: {Endpoint}", endpoint);

        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating resource at {Endpoint}", endpoint);
            return false;
        }
    }
}
