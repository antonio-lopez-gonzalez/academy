using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using TaskFlow.Core.DTOs;
using TaskFlow.Core.Entities;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Identity;
using TaskFlow.Api.Extensions;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TaskFlowDbContext _context;
    private readonly JwtService _jwt;
    private readonly PasswordHasher _hasher;

    public AuthController(TaskFlowDbContext context, JwtService jwt, PasswordHasher hasher)
    {
        _context = context;
        _jwt = jwt;
        _hasher = hasher;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest(new { message = "El email ya existe" });

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = _hasher.Hash(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponseDto
        {
            User = MapUser(user),
            Token = _jwt.GenerateToken(user)
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !_hasher.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Credenciales inválidas" });

        return Ok(new AuthResponseDto
        {
            User = MapUser(user),
            Token = _jwt.GenerateToken(user)
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.GetUserId();
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();
        return Ok(MapUser(user));
    }

    private static UserDto MapUser(User u) =>
        new() { Id = u.Id, Name = u.Name, Email = u.Email };
}
