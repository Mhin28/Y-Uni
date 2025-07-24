using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Repositories.ViewModels.AiModels;
using Services.Services.GeminiAIService;
using Services.Services.ContextService;
using Services.Services.ConflictDetectionService;
using Services.Services.AssignmentService;
using Services.Services.EventService;
using Services.Services.SubjectService;
using Services.Services.EventCategoryService;

namespace API.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize] // IMPORTANT: Secure this controller
public class AiController : ControllerBase
{
    private readonly IGeminiAIService _geminiService;
    private readonly IContextService _contextService;
    private readonly IConflictDetectionService _conflictService;
    private readonly IAssignmentService _assignmentService;
    private readonly IEventService _eventService;
    private readonly ISubjectService _subjectService;
    private readonly IEventCategoryService _eventCategoryService;

    public AiController(
        IGeminiAIService geminiService, 
        IContextService contextService, 
        IConflictDetectionService conflictService,
        IAssignmentService assignmentService,
        IEventService eventService,
        ISubjectService subjectService,
        IEventCategoryService eventCategoryService)
    {
        _geminiService = geminiService;
        _contextService = contextService;
        _conflictService = conflictService;
        _assignmentService = assignmentService;
        _eventService = eventService;
        _subjectService = subjectService;
        _eventCategoryService = eventCategoryService;
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
            
            if (request.SelectedOption.Type.Equals("assignment", StringComparison.OrdinalIgnoreCase))
            {
                // Find or create subject
                Guid? subjectId = null;
                if (!string.IsNullOrEmpty(request.SelectedOption.SubjectName))
                {
                    subjectId = await GetOrCreateSubjectId(request.SelectedOption.SubjectName, userId.Value);
                }

                // Create Assignment using existing service
                var assignmentModel = new Repositories.ViewModels.AssignmentModel.PostAssignmentModel
                {
                    Title = request.SelectedOption.Title,
                    Description = request.SelectedOption.Description ?? "",
                    DueDate = request.SelectedOption.DueDate,
                    PriorityId = GetPriorityId(request.SelectedOption.Priority),
                    EstimatedTime = request.SelectedOption.EstimatedTimeMinutes,
                    SubjectId = subjectId
                };
                
                var result = await _assignmentService.AddAsync(assignmentModel, userId.Value);
                return Ok(new { 
                    message = "Assignment created successfully!", 
                    assignmentId = result.Data,
                    type = "assignment"
                });
            }
            else if (request.SelectedOption.Type.Equals("event", StringComparison.OrdinalIgnoreCase))
            {
                // Find or create event category
                Guid? categoryId = null;
                if (!string.IsNullOrEmpty(request.SelectedOption.CategoryName))
                {
                    categoryId = await GetOrCreateCategoryId(request.SelectedOption.CategoryName, userId.Value);
                }

                // Create Event using existing service
                var eventModel = new Repositories.ViewModels.EventModel.PostEventModel
                {
                    Title = request.SelectedOption.Title,
                    Description = request.SelectedOption.Description ?? "",
                    StartDateTime = request.SelectedOption.DueDate,
                    EndDateTime = request.SelectedOption.DueDate.AddMinutes(request.SelectedOption.EstimatedTimeMinutes),
                    RecurrencePattern = "none",
                    EvCategoryId = categoryId
                };
                
                var result = await _eventService.AddAsync(eventModel, userId.Value);
                return Ok(new { 
                    message = "Event created successfully!", 
                    eventId = result.Data,
                    type = "event"
                });
            }
            else
            {
                return BadRequest(new { message = "Invalid option type. Must be 'assignment' or 'event'." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the item.", error = ex.Message });
        }
    }
    
    private byte GetPriorityId(string priority)
    {
        return priority.ToLower() switch
        {
            "low" => 1,
            "medium" => 2,
            "high" => 3,
            _ => 2 // Default to medium
        };
    }

    private async Task<Guid?> GetOrCreateSubjectId(string subjectName, Guid userId)
    {
        try
        {
            // First, try to find existing subject using service
            var existingSubjectsResult = await _subjectService.GetByUserIdAsync(userId);
            if (existingSubjectsResult.IsSuccess && existingSubjectsResult.Data != null)
            {
                var subjects = (List<Repositories.ViewModels.SubjectModel.SubjectModel>)existingSubjectsResult.Data;
                var existingSubject = subjects.FirstOrDefault(s => 
                    s.SubjectName.Equals(subjectName, StringComparison.OrdinalIgnoreCase));
                
                if (existingSubject != null)
                {
                    return existingSubject.SubjectId;
                }
            }

            // If not found, create new subject using service
            var newSubjectModel = new Repositories.ViewModels.SubjectModel.PostSubjectModel
            {
                SubjectName = subjectName,
                Description = $"Auto-created subject: {subjectName}"
            };

            var createResult = await _subjectService.AddAsync(newSubjectModel);
            if (createResult.IsSuccess && createResult.Data != null)
            {
                var createdSubject = (Repositories.ViewModels.SubjectModel.SubjectModel)createResult.Data;
                return createdSubject.SubjectId;
            }

            return null;
        }
        catch (Exception)
        {
            // If anything fails, return null (assignment will be created without subject)
            return null;
        }
    }

    private async Task<Guid?> GetOrCreateCategoryId(string categoryName, Guid userId)
    {
        try
        {
            // First, try to find existing category using service
            var existingCategoriesResult = await _eventCategoryService.GetByUserIdAsync(userId);
            if (existingCategoriesResult.IsSuccess && existingCategoriesResult.Data != null)
            {
                var categories = (List<Repositories.ViewModels.EventCategoryModel.EventCategoryModel>)existingCategoriesResult.Data;
                var existingCategory = categories.FirstOrDefault(c => 
                    c.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
                
                if (existingCategory != null)
                {
                    return existingCategory.EvCategoryId;
                }
            }

            // If not found, create new category using service
            var newCategoryModel = new Repositories.ViewModels.EventCategoryModel.PostEventCategoryModel
            {
                CategoryName = categoryName,
                Description = $"Auto-created category: {categoryName}"
            };

            var createResult = await _eventCategoryService.AddAsync(newCategoryModel);
            if (createResult.IsSuccess && createResult.Data != null)
            {
                var createdCategory = (Repositories.ViewModels.EventCategoryModel.EventCategoryModel)createResult.Data;
                return createdCategory.EvCategoryId;
            }

            return null;
        }
        catch (Exception)
        {
            // If anything fails, return null (event will be created without category)
            return null;
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