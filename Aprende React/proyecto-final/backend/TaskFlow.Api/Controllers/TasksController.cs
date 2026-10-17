using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Extensions;
using TaskFlow.Api.Hubs;
using TaskFlow.Core.DTOs;
using TaskFlow.Core.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId:guid}/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskFlowDbContext _context;
    private readonly IHubContext<TasksHub> _hub;

    public TasksController(TaskFlowDbContext context, IHubContext<TasksHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    private Guid CurrentUserId => User.GetUserId();

    private async Task<bool> CanAccessProject(Guid projectId)
    {
        return await _context.Projects.AnyAsync(p =>
            p.Id == projectId &&
            (p.OwnerId == CurrentUserId || p.Members.Any(m => m.UserId == CurrentUserId)));
    }

    [HttpGet]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        if (!await CanAccessProject(projectId)) return NotFound();

        var tasks = await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Subtasks)
            .Include(t => t.Comments)
            .ThenInclude(c => c.Author)
            .Select(t => Map(t))
            .ToListAsync();

        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid projectId, CreateTaskDto dto)
    {
        if (!await CanAccessProject(projectId)) return NotFound();

        var task = new TaskItem
        {
            ProjectId = projectId,
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            Labels = dto.Labels,
            DueDate = dto.DueDate
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var result = Map(task);
        await _hub.Clients.Group(projectId.ToString()).SendAsync("TaskCreated", result);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid projectId, Guid id, UpdateTaskDto dto)
    {
        if (!await CanAccessProject(projectId)) return NotFound();

        var task = await _context.Tasks.FindAsync(id);
        if (task == null || task.ProjectId != projectId) return NotFound();

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.Labels = dto.Labels;
        task.DueDate = dto.DueDate;

        await _context.SaveChangesAsync();

        var result = Map(task);
        await _hub.Clients.Group(projectId.ToString()).SendAsync("TaskUpdated", result);
        return Ok(result);
    }

    [HttpPut("{id:guid}/move")]
    public async Task<IActionResult> Move(Guid projectId, Guid id, MoveTaskDto dto)
    {
        if (!await CanAccessProject(projectId)) return NotFound();

        var task = await _context.Tasks.FindAsync(id);
        if (task == null || task.ProjectId != projectId) return NotFound();

        task.Status = dto.Status;
        await _context.SaveChangesAsync();

        var result = Map(task);
        await _hub.Clients.Group(projectId.ToString()).SendAsync("TaskMoved", result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid projectId, Guid id)
    {
        if (!await CanAccessProject(projectId)) return NotFound();

        var task = await _context.Tasks.FindAsync(id);
        if (task == null || task.ProjectId != projectId) return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        await _hub.Clients.Group(projectId.ToString()).SendAsync("TaskDeleted", id);
        return NoContent();
    }

    private static TaskDto Map(TaskItem t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Status = t.Status,
        Priority = t.Priority,
        Labels = t.Labels,
        DueDate = t.DueDate,
        ProjectId = t.ProjectId,
        CreatedAt = t.CreatedAt,
        Subtasks = t.Subtasks.Select(s => new SubtaskDto
        {
            Id = s.Id,
            Content = s.Content,
            Done = s.Done
        }).ToList(),
        Comments = t.Comments.Select(c => new CommentDto
        {
            Id = c.Id,
            Content = c.Content,
            AuthorId = c.AuthorId,
            AuthorName = c.Author.Name,
            CreatedAt = c.CreatedAt
        }).ToList()
    };
}
