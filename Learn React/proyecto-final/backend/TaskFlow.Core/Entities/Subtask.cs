namespace TaskFlow.Core.Entities;

public class Subtask
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool Done { get; set; }
    public Guid TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;
}
