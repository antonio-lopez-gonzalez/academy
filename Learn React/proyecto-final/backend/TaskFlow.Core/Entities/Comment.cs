namespace TaskFlow.Core.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
