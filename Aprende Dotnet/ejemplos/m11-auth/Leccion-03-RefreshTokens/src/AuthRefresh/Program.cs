// ===================================================================
// Lección 03 — Refresh Tokens
// ===================================================================
// PROBLEMA: El access token JWT expira rápido (típicamente 15-60 min).
//   Si expira, el usuario tendría que volver a hacer login.
//   Eso es mala experiencia de usuario.
//
// SOLUCIÓN: Refresh Token.
//   - Access token: vida corta (15 min), se envía en cada request.
//   - Refresh token: vida larga (7-30 días), se usa SOLO para obtener
//     un nuevo access token cuando el actual expira.
//
// FLUJO:
//   1. POST /login     → devuelve { access_token, refresh_token }
//   2. Cliente usa access_token en peticiones normales
//   3. Cuando access_token expira (401), el cliente hace:
//      POST /refresh   → envía refresh_token → recibe NUEVO access_token
//                           + NUEVO refresh_token (rotation)
//   4. POST /logout    → invalida el refresh_token
//
// ROTACIÓN (Refresh Token Rotation):
//   Cada vez que se usa un refresh token, se emite uno NUEVO
//   y el viejo se invalida. Esto previene el robo de refresh tokens:
//   - Si un atacante roba un refresh token,
//   - el usuario legítimo usará el suyo primero,
//   - invalidando el token robado.
//   - Cuando el atacante intente usar el token robado → será rechazado.
//
// ALMACENAMIENTO:
//   - Aquí usamos diccionario en memoria (solo para demo).
//   - En producción: tabla en base de datos (PostgreSQL/Redis).
//   - Los refresh tokens son opaque tokens (GUIDs aleatorios), no JWT.
//     No necesitan ser JWT porque solo se validan contra la BD.
// ===================================================================

using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ─── CONFIGURACIÓN ───────────────────────────────────────────────────────────
string secretKey = builder.Configuration["JwtSettings:SecretKey"]!;
string issuer = builder.Configuration["JwtSettings:Issuer"]!;
string audience = builder.Configuration["JwtSettings:Audience"]!;
int accessTokenExpirationMinutes = int.Parse(builder.Configuration["JwtSettings:ExpirationMinutes"]!);
TimeSpan refreshTokenLifetime = TimeSpan.FromDays(7); // El refresh token dura 7 días

// ─── AUTENTICACIÓN JWT ──────────────────────────────────────────────────────
byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

WebApplication app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// ─── ALMACENAMIENTO DE REFRESH TOKENS (SIMULADO) ────────────────────────────
// En producción: tabla en PostgreSQL con columns:
//   - Id (GUID), Username, TokenHash, ExpiresAt, RevokedAt, CreatedAt
//
// Aquí usamos ConcurrentDictionary para thread-safety.
ConcurrentDictionary<string, RefreshTokenEntry> refreshTokensStore = new();

// ─── USUARIOS SIMULADOS ─────────────────────────────────────────────────────
Dictionary<string, UserData> users = new()
{
    ["admin"] = new UserData("admin", BCrypt.Net.BCrypt.HashPassword("Admin123!"), "Admin"),
    ["user"] = new UserData("user", BCrypt.Net.BCrypt.HashPassword("User123!"), "User")
};

// ===================================================================
// ENDPOINTS
// ===================================================================

// POST /login — Autentica al usuario y devuelve access_token + refresh_token
app.MapPost("/login", [AllowAnonymous] (LoginRequest request) =>
{
    if (!users.TryGetValue(request.Username, out UserData? user))
        return Results.Json(new { error = "Credenciales inválidas." }, statusCode: 401);

    if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        return Results.Json(new { error = "Credenciales inválidas." }, statusCode: 401);

    // Generar access token (JWT, vida corta)
    string accessToken = GenerateAccessToken(user);

    // Generar refresh token (opaco, vida larga)
    string refreshToken = GenerateRefreshToken(user.Username);

    return Results.Ok(new
    {
        access_token = accessToken,
        refresh_token = refreshToken,
        token_type = "Bearer",
        expires_in = accessTokenExpirationMinutes * 60
    });
});

// POST /refresh — Intercambia un refresh_token válido por un nuevo access_token
// Este es el endpoint clave del flujo de refresh.
app.MapPost("/refresh", [AllowAnonymous] (RefreshRequest request) =>
{
    // 1. Buscar el refresh token en nuestro almacén
    if (!refreshTokensStore.TryGetValue(request.RefreshToken, out RefreshTokenEntry? entry))
    {
        return Results.Json(new { error = "Refresh token inválido." }, statusCode: 401);
    }

    // 2. Validar que no ha expirado
    if (entry.ExpiresAt < DateTime.UtcNow)
    {
        // Limpiar token expirado
        refreshTokensStore.TryRemove(request.RefreshToken, out _);
        return Results.Json(new { error = "Refresh token expirado." }, statusCode: 401);
    }

    // 3. Validar que no ha sido revocado
    if (entry.IsRevoked)
    {
        // ¡Posible robo de token! Alguien está usando un token ya revocado.
        // En producción: revocar TODOS los refresh tokens de este usuario
        // y forzar re-login.
        return Results.Json(new { error = "Refresh token revocado." }, statusCode: 401);
    }

    // 4. Buscar el usuario asociado al refresh token
    if (!users.TryGetValue(entry.Username, out UserData? user))
    {
        return Results.Json(new { error = "Usuario no encontrado." }, statusCode: 401);
    }

    // ─── ROTACIÓN: invalidar refresh token viejo, emitir uno nuevo ─────
    // Marcamos el token actual como revocado
    entry.Revoke();

    // Generamos nuevo access token
    string newAccessToken = GenerateAccessToken(user);

    // Generamos nuevo refresh token (rotación)
    string newRefreshToken = GenerateRefreshToken(user.Username);

    return Results.Ok(new
    {
        access_token = newAccessToken,
        refresh_token = newRefreshToken,
        token_type = "Bearer",
        expires_in = accessTokenExpirationMinutes * 60
    });
});

// POST /logout — Revoca el refresh token (el access token sigue válido hasta que expire)
app.MapPost("/logout", [AllowAnonymous] (LogoutRequest request) =>
{
    if (refreshTokensStore.TryGetValue(request.RefreshToken, out RefreshTokenEntry? entry))
    {
        entry.Revoke();
    }

    // Siempre devolvemos 200, incluso si el token no existe
    // (para no revelar información a atacantes)
    return Results.Ok(new { message = "Sesión cerrada correctamente." });
});

// GET /me — Endpoint protegido (solo access token válido)
app.MapGet("/me", [Authorize] (ClaimsPrincipal user) =>
{
    return Results.Ok(new
    {
        username = user.FindFirst(ClaimTypes.Name)?.Value,
        role = user.FindFirst(ClaimTypes.Role)?.Value
    });
});

app.Run();

// ─── FUNCIONES AUXILIARES ────────────────────────────────────────────────────

/// <summary>
/// Genera un access token JWT con vida corta.
/// </summary>
string GenerateAccessToken(UserData user)
{
    SymmetricSecurityKey key = new(keyBytes);
    SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

    Claim[] claims =
    [
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    ];

    JwtSecurityToken token = new(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

/// <summary>
/// Genera un refresh token opaco (GUID aleatorio criptográficamente seguro).
/// No es un JWT, es un identificador único que se almacena en BD.
/// </summary>
string GenerateRefreshToken(string username)
{
    // Usamos RandomNumberGenerator para generar bytes aleatorios criptográficos
    byte[] randomBytes = new byte[64];
    using RandomNumberGenerator rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomBytes);

    string refreshToken = Convert.ToBase64String(randomBytes);

    // Almacenamos en nuestro "almacén" simulado
    refreshTokensStore[refreshToken] = new RefreshTokenEntry(
        Token: refreshToken,
        Username: username,
        ExpiresAt: DateTime.UtcNow.Add(refreshTokenLifetime)
    );

    return refreshToken;
}

// ─── MODELOS ─────────────────────────────────────────────────────────────────

public record LoginRequest(string Username, string Password);
public record RefreshRequest(string RefreshToken);
public record LogoutRequest(string RefreshToken);
public record UserData(string Username, string PasswordHash, string Role);

/// <summary>
/// Entrada en el almacén de refresh tokens.
/// En producción sería una fila en tabla SQL.
/// </summary>
public class RefreshTokenEntry
{
    public string Token { get; init; }
    public string Username { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; init; }
    public DateTime? RevokedAt { get; private set; }
    public bool IsRevoked => RevokedAt is not null;

    public RefreshTokenEntry(string Token, string Username, DateTime ExpiresAt)
    {
        this.Token = Token;
        this.Username = Username;
        this.ExpiresAt = ExpiresAt;
    }

    /// <summary>
    /// Revoca este refresh token (lo marca como inválido).
    /// </summary>
    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }
}
