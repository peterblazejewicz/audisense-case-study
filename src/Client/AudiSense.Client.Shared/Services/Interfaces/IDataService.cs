namespace AudiSense.Client.Shared.Services.Interfaces;

/// <summary>
/// Generic interface for data service operations
/// </summary>
public interface IDataService
{
    /// <summary>
    /// Get data from API endpoint
    /// </summary>
    /// <typeparam name="T">Type of data to retrieve</typeparam>
    /// <param name="endpoint">API endpoint</param>
    /// <returns>Retrieved data or null</returns>
    Task<T?> GetAsync<T>(string endpoint) where T : class;

    /// <summary>
    /// Post data to API endpoint
    /// </summary>
    /// <typeparam name="TRequest">Type of request data</typeparam>
    /// <typeparam name="TResponse">Type of response data</typeparam>
    /// <param name="endpoint">API endpoint</param>
    /// <param name="data">Data to post</param>
    /// <returns>Response data or null</returns>
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        where TRequest : class
        where TResponse : class;

    /// <summary>
    /// Update data at API endpoint
    /// </summary>
    /// <typeparam name="T">Type of data to update</typeparam>
    /// <param name="endpoint">API endpoint</param>
    /// <param name="data">Data to update</param>
    /// <returns>Success status</returns>
    Task<bool> UpdateAsync<T>(string endpoint, T data) where T : class;

    /// <summary>
    /// Delete data at API endpoint
    /// </summary>
    /// <param name="endpoint">API endpoint</param>
    /// <returns>Success status</returns>
    Task<bool> DeleteAsync(string endpoint);
}
