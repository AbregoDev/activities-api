using Application.Activities;
using Application.Activities.Createactivity;
using Application.Activities.GetActivity;
using Application.Activities.UpdateActivity;
using Application.Activities.DeleteActivity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Presentation.Contracts;
using Presentation.Extensions;

namespace Presentation.Controllers;

[ApiController]
[Route("/api/[Controller]")]
public sealed class ActivitiesController(
    CreateActivityUseCase createActivityUseCase,
    GetActivitiesUseCase getActivitiesUseCase,
    GetActivitiesInRangeUseCase getActivitiesInRangeUseCase,
    GetActivityByIdUseCase getActivityByIdUseCase,
    UpdateActivityUseCase updateActivityUseCase,
    DeleteActivityUseCase deleteActivityUseCase) : ControllerBase
{
    [HttpPost(Name = "CreateActivity")]
    public async Task<IActionResult> CreateActivity([FromBody] ActivityDataRequest request)
    {
        var activity = await createActivityUseCase.ExecuteAsync(
            request.Name, request.Date, request.StartTime, request.EndTime);

        return Created($"/activity/{activity.Id}", activity);
    }

    [HttpGet(Name = "GetActivities")]
    public async Task<IActionResult> GetActivities()
    {
        var activities = await getActivitiesUseCase.ExecuteAsync();
        return Ok(activities);
    }

    [HttpGet("range", Name = "GetActivitiesInRange")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [ProducesResponseType<ActivityResponse[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ActivityResponse[]>> GetActivitiesInRange(
        [FromQuery, BindRequired] DateOnly startDate,
        [FromQuery, BindRequired] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        if (startDate > endDate)
        {
            ModelState.AddModelError(nameof(endDate), "End date must be on or after start date.");
            return ValidationProblem(ModelState);
        }

        var activities = await getActivitiesInRangeUseCase.ExecuteAsync(
            startDate, endDate, cancellationToken);
        return Ok(activities.Select(activity => new ActivityResponse(
            activity.Id,
            activity.Name,
            activity.CategoryId,
            activity.Date,
            activity.StartTime,
            activity.EndTime,
            activity.IsCompleted)).ToArray());
    }

    [HttpGet("{activityId}", Name = "GetActivityById")]
    public async Task<IActionResult> GetActivityById(Guid activityId)
    {
        var activity = await getActivityByIdUseCase.ExecuteAsync(activityId);
        return Ok(activity);
    }

    [HttpPatch("{activityId}", Name = "UpdateActivity")]
    public async Task<IActionResult> UpdateActivity(Guid activityId, [FromBody] ActivityUpdatedDataRequest request)
    {
        var newActivityData = request.ToActivityDataToUpdate();
        var activity = await updateActivityUseCase.ExecuteAsync(activityId, newActivityData);
    
        return Ok(activity);
    }

    [HttpDelete("{activityId}", Name = "DeleteActivity")]
    public async Task<IActionResult> DeleteActivity(Guid activityId)
    {
        await deleteActivityUseCase.ExecuteAsync(activityId);
        return NoContent();
    }
}

public sealed record ActivityDataRequest(
    string Name,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    Guid? CategoryId);

public sealed record ActivityUpdatedDataRequest(
    string? Name,
    DateOnly? Date,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    Guid? CategoryId,
    bool? IsCompleted);
