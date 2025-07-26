using AudiSense.Contracts.Interfaces;
using AudiSense.Contracts.Requests;
using AudiSense.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace AudiSense.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HearingTestsController : ControllerBase
{
    private readonly IHearingTestService _hearingTestService;
    private readonly ILogger<HearingTestsController> _logger;

    public HearingTestsController(IHearingTestService hearingTestService, ILogger<HearingTestsController> logger)
    {
        _hearingTestService = hearingTestService;
        _logger = logger;
    }

    /// <summary>
    /// Get all hearing tests
    /// </summary>
    /// <returns>List of all hearing tests</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HearingTestResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HearingTestResponse>>> GetAllHearingTests()
    {
        _logger.LogInformation("Retrieving all hearing tests");
        var hearingTests = await _hearingTestService.GetAllHearingTestsAsync();
        return Ok(hearingTests);
    }

    /// <summary>
    /// Get a specific hearing test by ID
    /// </summary>
    /// <param name="id">The hearing test ID</param>
    /// <returns>The hearing test if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HearingTestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HearingTestResponse>> GetHearingTest(int id)
    {
        _logger.LogInformation("Retrieving hearing test with ID: {Id}", id);
        var hearingTest = await _hearingTestService.GetHearingTestByIdAsync(id);

        if (hearingTest == null)
        {
            _logger.LogWarning("Hearing test with ID: {Id} not found", id);
            return NotFound();
        }

        return Ok(hearingTest);
    }

    /// <summary>
    /// Create a new hearing test
    /// </summary>
    /// <param name="request">The hearing test data</param>
    /// <returns>The created hearing test</returns>
    [HttpPost]
    [ProducesResponseType(typeof(HearingTestResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HearingTestResponse>> CreateHearingTest(HearingTestRequest request)
    {
        _logger.LogInformation("Creating new hearing test for tester: {TesterName}", request.TesterName);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdTest = await _hearingTestService.CreateHearingTestAsync(request);
        return CreatedAtAction(nameof(GetHearingTest), new { id = createdTest.Id }, createdTest);
    }

    /// <summary>
    /// Update an existing hearing test
    /// </summary>
    /// <param name="id">The hearing test ID</param>
    /// <param name="request">The updated hearing test data</param>
    /// <returns>The updated hearing test</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(HearingTestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HearingTestResponse>> UpdateHearingTest(int id, HearingTestRequest request)
    {
        _logger.LogInformation("Updating hearing test with ID: {Id}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedTest = await _hearingTestService.UpdateHearingTestAsync(id, request);

        if (updatedTest == null)
        {
            _logger.LogWarning("Hearing test with ID: {Id} not found for update", id);
            return NotFound();
        }

        return Ok(updatedTest);
    }

    /// <summary>
    /// Delete a hearing test
    /// </summary>
    /// <param name="id">The hearing test ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHearingTest(int id)
    {
        _logger.LogInformation("Deleting hearing test with ID: {Id}", id);

        var result = await _hearingTestService.DeleteHearingTestAsync(id);

        if (!result)
        {
            _logger.LogWarning("Hearing test with ID: {Id} not found for deletion", id);
            return NotFound();
        }

        return NoContent();
    }
}
