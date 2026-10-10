using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Extensions;
using TaskFlow.Core.DTOs;
using TaskFlow.Core.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks/{taskId:guid}/[controller]")]
public class SubtasksController : ControllerBase
{
    private readonly TaskFlowDbContext _context;

    public SubtasksController(TaskFlowDbContext context)
    {
        _context = context;
    }

    private Guid CurrentUserId => User.GetUserId();

    private async Task<bool> CanAccessTask(Guid taskId)
    {
        return await _context.Tasks.AnyAsync(t =>
            t.Id == taskId &&
            (t.Project.OwnerId == CurrentUserId ||
             t.Project.Members.Any(m => m.UserId == CurrentUserId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid taskId, CreateSubtaskDto dto)
    {
        if (!await CanAccessTask(taskId)) return NotFound();

        var subtask = new Subtask
        {
            TaskItemId = taskId,
            Content = dto.Content
        };

        _context.Subtasks.Add(subtask);
        await _context.SaveChangesAsync();

        return Ok(new SubtaskDto
        {
            Id = subtask.Id,
            Content = subtask.Content,
            Done = subtask.Done
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid taskId, Guid id, UpdateSubtaskDto dto)
    {
        if (!await CanAccessTask(taskId)) return NotFound();

        var subtask = await _context.Subtasks.FindAsync(id);
        if (subtask == null || subtask.TaskItemId != taskId) return NotFound();

        subtask.Done = dto.Done;
        await _context.SaveChangesAsync();

        return Ok(new SubtaskDto
        {
            Id = subtask.Id,
            Content = subtask.Content,
            Done = subtask.Done
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid taskId, Guid id)
    {
        if (!await CanAccessTask(taskId)) return NotFound();

        var subtask = await _context.Subtasks.FindAsync(id);
        if (subtask == null || subtask.TaskItemId != taskId) return NotFound();

        _context.Subtasks.Remove(subtask);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
