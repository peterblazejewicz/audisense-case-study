using AudiSense.Contracts.Requests;
using AudiSense.Contracts.Responses;

namespace AudiSense.Wpf.Core.Services;

public interface IHearingTestApiService
{
    Task<IEnumerable<HearingTestResponse>> GetAllHearingTestsAsync();
    Task<HearingTestResponse?> GetHearingTestByIdAsync(int id);
    Task<HearingTestResponse?> CreateHearingTestAsync(HearingTestRequest request);
    Task<HearingTestResponse?> UpdateHearingTestAsync(int id, HearingTestRequest request);
    Task<bool> DeleteHearingTestAsync(int id);
}
