using AudiSense.Contracts.Interfaces;
using AudiSense.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace AudiSense.Infrastructure.Persistence;

public class HearingTestRepository : IHearingTestRepository
{
    private readonly AudiSenseDbContext _context;

    public HearingTestRepository(AudiSenseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HearingTest>> GetAllAsync()
    {
        return await _context.HearingTests.ToListAsync();
    }

    public async Task<HearingTest?> GetByIdAsync(int id)
    {
        return await _context.HearingTests.FindAsync(id);
    }

    public async Task<HearingTest> CreateAsync(HearingTest hearingTest)
    {
        _context.HearingTests.Add(hearingTest);
        await _context.SaveChangesAsync();
        return hearingTest;
    }

    public async Task<HearingTest?> UpdateAsync(int id, HearingTest hearingTest)
    {
        var existingTest = await _context.HearingTests.FindAsync(id);
        if (existingTest == null)
            return null;

        existingTest.TesterName = hearingTest.TesterName;
        existingTest.DateConducted = hearingTest.DateConducted;
        existingTest.Result = hearingTest.Result;

        await _context.SaveChangesAsync();
        return existingTest;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var hearingTest = await _context.HearingTests.FindAsync(id);
        if (hearingTest == null)
            return false;

        _context.HearingTests.Remove(hearingTest);
        await _context.SaveChangesAsync();
        return true;
    }
}
