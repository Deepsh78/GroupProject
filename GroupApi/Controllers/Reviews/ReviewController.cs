using GroupApi.DTOs.Reviews;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _reviewService.GetAllAsync();
        if (result.IsSuccess)
            return Ok(result.Data);

        return StatusCode((int)result.Error!.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _reviewService.GetByIdAsync(id);
        if (result.IsSuccess)
            return Ok(result.Data);

        return StatusCode((int)result.Error!.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReviewCreateDto dto)
    {
        var created = await _reviewService.CreateAsync(dto);
        if (created.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = created.Data!.ReviewId }, created.Data);

        return StatusCode((int)created.Error!.StatusCode, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewDto dto)
    {
        var updated = await _reviewService.UpdateAsync(id, dto);
        if (updated.IsSuccess)
            return Ok(updated.Data);

        return StatusCode((int)updated.Error!.StatusCode, updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _reviewService.DeleteAsync(id);
        if (result.IsSuccess)
            return NoContent();

        return StatusCode((int)result.Error!.StatusCode, result);
    }
}
