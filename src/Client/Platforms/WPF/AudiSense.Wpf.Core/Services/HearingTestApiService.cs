using AudiSense.Client.Shared.Services.Interfaces;
using AudiSense.Contracts.Requests;
using AudiSense.Contracts.Responses;

using Microsoft.Extensions.Logging;

namespace AudiSense.Wpf.Core.Services;

public class HearingTestApiService : IHearingTestApiService
{
    private readonly IDataService _dataService;
    private readonly ILogger<HearingTestApiService> _logger;
    private const string BaseEndpoint = "api/hearingtests";

    public HearingTestApiService(IDataService dataService, ILogger<HearingTestApiService> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    public async Task<IEnumerable<HearingTestResponse>> GetAllHearingTestsAsync()
    {
        _logger.LogInformation("Fetching all hearing tests");

        try
        {
            var result = await _dataService.GetAsync<IEnumerable<HearingTestResponse>>(BaseEndpoint);
            return result ?? new List<HearingTestResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching hearing tests");
            return new List<HearingTestResponse>();
        }
    }

    public async Task<HearingTestResponse?> GetHearingTestByIdAsync(int id)
    {
        _logger.LogInformation("Fetching hearing test with ID: {Id}", id);

        try
        {
            return await _dataService.GetAsync<HearingTestResponse>($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching hearing test with ID: {Id}", id);
            return null;
        }
    }

    public async Task<HearingTestResponse?> CreateHearingTestAsync(HearingTestRequest request)
    {
        _logger.LogInformation("Creating new hearing test for tester: {TesterName}", request.TesterName);

        try
        {
            return await _dataService.PostAsync<HearingTestRequest, HearingTestResponse>(BaseEndpoint, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hearing test");
            return null;
        }
    }

    public async Task<HearingTestResponse?> UpdateHearingTestAsync(int id, HearingTestRequest request)
    {
        _logger.LogInformation("Updating hearing test with ID: {Id}", id);

        try
        {
            var success = await _dataService.UpdateAsync($"{BaseEndpoint}/{id}", request);
            if (success)
            {
                return await GetHearingTestByIdAsync(id);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hearing test with ID: {Id}", id);
            return null;
        }
    }

    public async Task<bool> DeleteHearingTestAsync(int id)
    {
        _logger.LogInformation("Deleting hearing test with ID: {Id}", id);

        try
        {
            return await _dataService.DeleteAsync($"{BaseEndpoint}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hearing test with ID: {Id}", id);
            return false;
        }
    }
}
