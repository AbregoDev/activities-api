using Application.Activities;
using Application.Activities.Createactivity;
using Application.Activities.GetActivity;
using Application.Activities.UpdateActivity;
using Application.Activities.DeleteActivity;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("/api/[Controller]")]
public sealed class ActivitiesController(
    CreateActivityUseCase createActivityUseCase,
    GetActivitiesUseCase getActivitiesUseCase,
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

    [HttpGet("{activityId}", Name = "GetActivityById")]
    public async Task<IActionResult> GetActivityById(Guid activityId)
    {
        var activity = await getActivityByIdUseCase.ExecuteAsync(activityId);
        return Ok(activity);
    }

    [HttpPatch("{activityId}", Name = "UpdateActivity")]
    public async Task<IActionResult> UpdateActivity(Guid activityId, [FromBody] ActivityUpdatedDataRequest request)
    {
        var activity = await updateActivityUseCase.ExecuteAsync(activityId, new ActivityDataToUpdate(
            request.Name, request.Date, request.StartTime, request.EndTime));
    
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
    TimeOnly EndTime);

public sealed record ActivityUpdatedDataRequest(
    string? Name,
    DateOnly? Date,
    TimeOnly? StartTime,
    TimeOnly? EndTime);
