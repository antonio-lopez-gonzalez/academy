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
public class CommentsController : ControllerBase
{
    private readonly TaskFlowDbContext _context;

    public CommentsController(TaskFlowDbContext context)
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

    [HttpGet]
    public async Task<IActionResult> GetByTask(Guid taskId)
    {
        if (!await CanAccessTask(taskId)) return NotFound();

        var comments = await _context.Comments
            .Where(c => c.TaskItemId == taskId)
            .Include(c => c.Author)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                AuthorId = c.AuthorId,
                AuthorName = c.Author.Name,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(comments);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid taskId, CreateCommentDto dto)
    {
        if (!await CanAccessTask(taskId)) return NotFound();

        var comment = new Comment
        {
            TaskItemId = taskId,
            AuthorId = CurrentUserId,
            Content = dto.Content
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        await _context.Entry(comment)
            .Reference(c => c.Author)
            .LoadAsync();

        return Ok(new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            AuthorId = comment.AuthorId,
            AuthorName = comment.Author!.Name,
            CreatedAt = comment.CreatedAt
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid taskId, Guid id)
    {
        if (!await CanAccessTask(taskId)) return NotFound();

        var comment = await _context.Comments.FindAsync(id);
        if (comment == null || comment.TaskItemId != taskId || comment.AuthorId != CurrentUserId)
            return NotFound();

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
