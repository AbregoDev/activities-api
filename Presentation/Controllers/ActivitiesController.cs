using Application.Activities.Createactivity;
using Application.Activities.GetActivity;
using Application.Activities.UpdateActivity;
using Application.Activities.DeleteActivity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Presentation.Contracts.Activities;
using Presentation.Extensions;

namespace Presentation.Controllers;

[ApiController]
[Route("/api/[Controller]")]
public sealed class ActivitiesController(
    CreateActivityUseCase createActivityUseCase,
    GetActivitiesUseCase getActivitiesUseCase,
    GetActivitiesByMonthUseCase getActivitiesByMonthUseCase,
    GetActivityByIdUseCase getActivityByIdUseCase,
    UpdateActivityUseCase updateActivityUseCase,
    DeleteActivityUseCase deleteActivityUseCase) : ControllerBase
{
    [HttpPost(Name = "CreateActivity")]
    public async Task<ActionResult<ActivityResponse>> CreateActivity(
        [FromBody] CreateActivityRequest request,
        CancellationToken ct)
    {
        var activity = await createActivityUseCase.ExecuteAsync(
            request.Name, request.Date, request.StartTime, request.EndTime, ct);

        return Created($"/activity/{activity.Id}", activity.ToActivityResponse());
    }

    [HttpGet(Name = "GetActivities")]
    public async Task<ActionResult<ActivityResponse[]>> GetActivities(CancellationToken ct)
    {
        var activities = await getActivitiesUseCase.ExecuteAsync(ct);
        return Ok(activities.Select(activity => activity.ToActivityResponse()).ToArray());
    }

    [HttpGet("GetActivitiesByMonth", Name = "GetActivitiesByMonth")]
    [EndpointSummary("Get activities for a calendar month")]
    [EndpointDescription("Returns activities ordered by date, start time, and ID. Month is 1-indexed (1-12).")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [ProducesResponseType<ActivityResponse[]>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ActivityResponse[]>> GetActivitiesByMonth(
        [FromQuery, BindRequired, Range(1, 9999)] int year,
        [FromQuery, BindRequired, Range(1, 12)] int month,
        CancellationToken ct)
    {
        var activities = await getActivitiesByMonthUseCase.ExecuteAsync(year, month, ct);
        return Ok(activities.Select(activity => activity.ToActivityResponse()).ToArray());
    }

    [HttpGet("{activityId}", Name = "GetActivityById")]
    public async Task<ActionResult<ActivityResponse>> GetActivityById(
        Guid activityId,
        CancellationToken ct)
    {
        var activity = await getActivityByIdUseCase.ExecuteAsync(activityId, ct);
        return Ok(activity.ToActivityResponse());
    }

    [HttpPatch("{activityId}", Name = "UpdateActivity")]
    public async Task<ActionResult<ActivityResponse>> UpdateActivity(
        Guid activityId,
        [FromBody] UpdateActivityRequest request,
        CancellationToken ct)
    {
        var newActivityData = request.ToActivityDataToUpdate();
        var activity = await updateActivityUseCase.ExecuteAsync(activityId, newActivityData, ct);

        return Ok(activity.ToActivityResponse());
    }

    [HttpDelete("{activityId}", Name = "DeleteActivity")]
    public async Task<IActionResult> DeleteActivity(
        Guid activityId,
        CancellationToken ct)
    {
        await deleteActivityUseCase.ExecuteAsync(activityId, ct);
        return NoContent();
    }
}
