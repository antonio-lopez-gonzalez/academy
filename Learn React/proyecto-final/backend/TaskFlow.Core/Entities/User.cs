namespace TaskFlow.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Project> Projects { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
    public List<ProjectMember> Memberships { get; set; } = [];
}
