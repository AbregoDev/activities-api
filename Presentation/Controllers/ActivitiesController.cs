using Application.Activities.Createactivity;
using Application.Activities.GetActivity;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("/api/[Controller]")]
public sealed class ActivitiesController(
    CreateActivityUseCase createActivityUseCase,
    GetActivitiesUseCase getActivitiesUseCase,
    GetActivityByIdUseCase getActivityByIdUseCase) : ControllerBase
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
}

public sealed record ActivityDataRequest(
    string Name,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime);
