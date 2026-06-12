namespace Backend.Api.Models;

public class TodoItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsDone { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
