using AudiSense.Domain.Entities;

namespace AudiSense.Contracts.Interfaces;

public interface IHearingTestRepository
{
    Task<IEnumerable<HearingTest>> GetAllAsync();
    Task<HearingTest?> GetByIdAsync(int id);
    Task<HearingTest> CreateAsync(HearingTest hearingTest);
    Task<HearingTest?> UpdateAsync(int id, HearingTest hearingTest);
    Task<bool> DeleteAsync(int id);
}
