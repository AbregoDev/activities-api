namespace Presentation.Contracts.Activities;

public sealed record UpdateActivityRequest(
    string? Name,
    DateOnly? Date,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    Guid? CategoryId,
    bool? IsCompleted);
