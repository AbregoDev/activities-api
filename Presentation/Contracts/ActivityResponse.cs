namespace Presentation.Contracts;

public sealed record ActivityResponse(
    Guid Id,
    string Name,
    Guid? CategoryId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsCompleted);
