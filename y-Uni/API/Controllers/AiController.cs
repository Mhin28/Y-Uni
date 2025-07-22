using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using API.DTOs.Ai;
using Services.Services.GeminiAIService;
using Services.Services.ContextService;
using Services.Services.ConflictDetectionService;

namespace API.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize] // IMPORTANT: Secure this controller
public class AiController : ControllerBase
{
    private readonly IGeminiAIService _geminiService;
    private readonly IContextService _contextService;
    private readonly IConflictDetectionService _conflictService;

    public AiController(
        IGeminiAIService geminiService, 
        IContextService contextService, 
        IConflictDetectionService conflictService)
    {
        _geminiService = geminiService;
        _contextService = contextService;
        _conflictService = conflictService;
    }

    [HttpPost("generate-options")]
    public async Task<IActionResult> GenerateCreationOptions([FromBody] ChatCreateRequest request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized();

            // 1. Get relevant context for the AI
            var context = await _contextService.GetRelevantContext(request.Message, userId.Value);

            // 2. Call Gemini to get structured options
            var aiOptions = await _geminiService.GenerateOptions(request.Message, context);
            if (!aiOptions.Any())
            {
                return BadRequest(new { message = "The AI could not generate options based on your request. Please try rephrasing." });
            }

            // 3. Check for conflicts
            var optionsWithConflicts = await _conflictService.CheckConflicts(aiOptions, userId.Value);

            // 4. Return the options to the client
            return Ok(new ChatCreateResponse
            {
                Options = optionsWithConflicts,
                ConversationId = Guid.NewGuid().ToString() // Generate a new ID for this interaction
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while generating options.", error = ex.Message });
        }
    }

    [HttpPost("create-selected")]
    public async Task<IActionResult> CreateFromSelectedOption([FromBody] CreateSelectedRequest request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized();
            
            // TODO: Use your existing services to create the item in the database
            // This will be implemented once we integrate with existing Assignment/Event services
            
            return Ok(new { message = "Feature coming soon - will integrate with existing services", option = request.SelectedOption });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the item.", error = ex.Message });
        }
    }

    private Guid? GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirstValue("userid"); // Based on your JWT token structure
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }
}