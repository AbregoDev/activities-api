namespace Domain.Entities;

public class Activity
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; }
    public Guid? CategoryId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsCompleted { get; set; } = false;

    public Activity(
        string name,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? category = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Task name cannot be empty.", nameof(name));

        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time.", nameof(startTime));

        Name = name;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        CategoryId = category;
    }
}
