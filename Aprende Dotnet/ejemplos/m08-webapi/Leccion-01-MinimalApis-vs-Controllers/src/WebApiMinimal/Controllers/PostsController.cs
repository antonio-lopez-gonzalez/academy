using Microsoft.AspNetCore.Mvc;
using WebApiMinimal.Models;

// ─────────────────────────────────────────────────────────────
// Controlador que maneja el CRUD de posts con el paradigma
// de clases + atributos (enfoque Controller-based).
// Equivale exactamente a los endpoints Minimal API de Program.cs.
// ─────────────────────────────────────────────────────────────
namespace WebApiMinimal.Controllers;

/// <summary>
/// Controlador que maneja el CRUD de posts.
/// </summary>
[ApiController]
[Route("api/v1/controller-posts")]
public class PostsController : ControllerBase
{
    private static readonly List<Post> _posts = new()
    {
        new Post(1, "Desde Controller", "Post creado desde un controlador", DateTime.UtcNow),
        new Post(2, "Otro post", "Contenido de prueba", DateTime.UtcNow),
    };

    /// <summary>GET /api/v1/controller-posts</summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(_posts);

    /// <summary>GET /api/v1/controller-posts/{id}</summary>
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        Post? post = _posts.Find(p => p.Id == id);
        if (post is null)
            return NotFound(new { Message = $"Post con ID {id} no encontrado" });
        return Ok(post);
    }

    /// <summary>POST /api/v1/controller-posts</summary>
    [HttpPost]
    public IActionResult Create([FromBody] Post newPost)
    {
        int newId = _posts.Max(p => p.Id) + 1;
        Post post = newPost with { Id = newId, CreatedAt = DateTime.UtcNow };
        _posts.Add(post);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    /// <summary>PUT /api/v1/controller-posts/{id}</summary>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Post updatedPost)
    {
        int index = _posts.FindIndex(p => p.Id == id);
        if (index == -1)
            return NotFound(new { Message = $"Post con ID {id} no encontrado" });
        _posts[index] = updatedPost with { Id = id, CreatedAt = _posts[index].CreatedAt };
        return Ok(_posts[index]);
    }

    /// <summary>DELETE /api/v1/controller-posts/{id}</summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        int removed = _posts.RemoveAll(p => p.Id == id);
        if (removed == 0)
            return NotFound(new { Message = $"Post con ID {id} no encontrado" });
        return NoContent();
    }
}
