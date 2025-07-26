using AudiSense.Contracts.Interfaces;
using AudiSense.Contracts.Requests;
using AudiSense.Contracts.Responses;
using AudiSense.Domain.Entities;

namespace AudiSense.Application.Services;

public class HearingTestService : IHearingTestService
{
    private readonly IHearingTestRepository _repository;

    public HearingTestService(IHearingTestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<HearingTestResponse>> GetAllHearingTestsAsync()
    {
        var hearingTests = await _repository.GetAllAsync();
        return hearingTests.Select(MapToResponse);
    }

    public async Task<HearingTestResponse?> GetHearingTestByIdAsync(int id)
    {
        var hearingTest = await _repository.GetByIdAsync(id);
        return hearingTest != null ? MapToResponse(hearingTest) : null;
    }

    public async Task<HearingTestResponse> CreateHearingTestAsync(HearingTestRequest request)
    {
        var hearingTest = new HearingTest
        {
            TesterName = request.TesterName,
            DateConducted = request.DateConducted,
            Result = request.Result
        };

        var createdTest = await _repository.CreateAsync(hearingTest);
        return MapToResponse(createdTest);
    }

    public async Task<HearingTestResponse?> UpdateHearingTestAsync(int id, HearingTestRequest request)
    {
        var hearingTest = new HearingTest
        {
            TesterName = request.TesterName,
            DateConducted = request.DateConducted,
            Result = request.Result
        };

        var updatedTest = await _repository.UpdateAsync(id, hearingTest);
        return updatedTest != null ? MapToResponse(updatedTest) : null;
    }

    public async Task<bool> DeleteHearingTestAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static HearingTestResponse MapToResponse(HearingTest hearingTest)
    {
        return new HearingTestResponse
        {
            Id = hearingTest.Id,
            TesterName = hearingTest.TesterName,
            DateConducted = hearingTest.DateConducted,
            Result = hearingTest.Result
        };
    }
}
