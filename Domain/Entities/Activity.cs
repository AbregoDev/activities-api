namespace Domain.Entities;

public class Activity
{
    public Guid Id { get; }
    public string Name { get; set; }
    public Guid? CategoryId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsCompleted { get; set; }

    public Activity(
        string name,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? categoryId = null,
        bool isCompleted = false)
        : this(Guid.NewGuid(), name, date, startTime, endTime, categoryId, isCompleted)
    {
    }

    public Activity(
        Guid id,
        string name,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? categoryId = null,
        bool isCompleted = false)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Activity ID cannot be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Task name cannot be empty.", nameof(name));

        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time.", nameof(startTime));

        Id = id;
        Name = name;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        CategoryId = categoryId;
        IsCompleted = isCompleted;
    }
}
