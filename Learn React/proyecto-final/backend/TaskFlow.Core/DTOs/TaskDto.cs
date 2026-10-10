namespace TaskFlow.Core.DTOs;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "todo";
    public string Priority { get; set; } = "medium";
    public List<string> Labels { get; set; } = [];
    public DateTime? DueDate { get; set; }
}

public class UpdateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "todo";
    public string Priority { get; set; } = "medium";
    public List<string> Labels { get; set; } = [];
    public DateTime? DueDate { get; set; }
}

public class MoveTaskDto
{
    public string Status { get; set; } = string.Empty;
}

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public List<string> Labels { get; set; } = [];
    public DateTime? DueDate { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<SubtaskDto> Subtasks { get; set; } = [];
    public List<CommentDto> Comments { get; set; } = [];
}

public class SubtaskDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool Done { get; set; }
}

public class CreateSubtaskDto
{
    public string Content { get; set; } = string.Empty;
}

public class UpdateSubtaskDto
{
    public bool Done { get; set; }
}
