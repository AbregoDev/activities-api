namespace Presentation.Contracts.Activities;

public sealed record CreateActivityRequest(
    string Name,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    Guid? CategoryId);
