namespace TaskFlow.Core.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "todo";
    public string Priority { get; set; } = "medium";
    public List<string> Labels { get; set; } = [];
    public DateTime? DueDate { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Subtask> Subtasks { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
}
