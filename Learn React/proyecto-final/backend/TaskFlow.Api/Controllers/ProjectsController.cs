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
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly TaskFlowDbContext _context;

    public ProjectsController(TaskFlowDbContext context)
    {
        _context = context;
    }

    private Guid CurrentUserId => User.GetUserId();

    private IQueryable<Project> AccessibleProjects => _context.Projects
        .Where(p => p.OwnerId == CurrentUserId ||
                    p.Members.Any(m => m.UserId == CurrentUserId));

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var projects = await AccessibleProjects
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OwnerId = p.OwnerId,
                CreatedAt = p.CreatedAt,
                TasksCount = p.Tasks.Count,
                MembersCount = p.Members.Count + 1
            })
            .ToListAsync();
        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var project = await AccessibleProjects
            .Include(p => p.Tasks)
            .ThenInclude(t => t.Subtasks)
            .Include(p => p.Tasks)
            .ThenInclude(t => t.Comments)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null) return NotFound();
        return Ok(Map(project));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectDto dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerId = CurrentUserId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, Map(project));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProjectDto dto)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null || project.OwnerId != CurrentUserId)
            return NotFound();

        project.Name = dto.Name;
        project.Description = dto.Description;
        await _context.SaveChangesAsync();
        return Ok(Map(project));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null || project.OwnerId != CurrentUserId)
            return NotFound();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static ProjectDto Map(Project p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        OwnerId = p.OwnerId,
        CreatedAt = p.CreatedAt,
        TasksCount = p.Tasks.Count,
        MembersCount = p.Members.Count + 1
    };
}
