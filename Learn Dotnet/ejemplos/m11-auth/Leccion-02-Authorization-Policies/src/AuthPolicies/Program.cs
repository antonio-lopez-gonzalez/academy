// ===================================================================
// Lección 02 — Authorization Policies (Roles, Claims, Policies)
// ===================================================================
// Roles vs Claims vs Policies:
//
//   ROLES (basado en roles):
//     [Authorize(Roles = "Admin")]
//     Ventaja: Simple, fácil de entender.
//     Desventaja: Añadir nuevo rol requiere deploy. Poco granular.
//     Ejemplo: "Admin", "Editor", "Author"
//
//   CLAIMS (basado en afirmaciones):
//     [Authorize(Policy = "RequireEmailVerified")]
//     Se define: p.RequireClaim("email_verified", "true")
//     Ventaja: Más granular que roles. No requiere nuevo rol.
//     Ejemplo: email_verified, subscription_level, department
//
//   POLICIES (basado en políticas personalizadas):
//     IAuthorizationRequirement + AuthorizationHandler<T>
//     Ventaja: Lógica de autorización compleja (edad, horario, etc.).
//     Permite inyectar dependencias en el handler.
//     Ejemplo: "AtLeast21YearsOld", "BusinessHoursOnly"
//
// ¿Cuándo usar cada uno?
//   - Roles: permisos simples y estables (Admin vs User)
//   - Claims: atributos del usuario que cambian (email_verified, premium)
//   - Policies: lógica de negocio compleja (horario laboral, límite de presupuesto)
// ===================================================================

using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ─── CONFIGURACIÓN JWT ──────────────────────────────────────────────────────
string secretKey = builder.Configuration["JwtSettings:SecretKey"]!;
string issuer = builder.Configuration["JwtSettings:Issuer"]!;
string audience = builder.Configuration["JwtSettings:Audience"]!;
int expirationMinutes = int.Parse(builder.Configuration["JwtSettings:ExpirationMinutes"]!);

byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);

// ─── AUTENTICACIÓN ──────────────────────────────────────────────────────────
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

// ─── AUTORIZACIÓN CON POLICIES ──────────────────────────────────────────────
builder.Services.AddAuthorization(opciones =>
{
    // ── Política basada en CLAIM ─────────────────────────────────────────
    // Requiere que el token tenga un claim "email_verified" con valor "true"
    opciones.AddPolicy("RequireEmailVerified", policy =>
        policy.RequireClaim("email_verified", "true"));

    // ── Política basada en ROL ───────────────────────────────────────────
    // Equivalente a [Authorize(Roles = "Admin,Editor")]
    opciones.AddPolicy("RequireAdminOrEditor", policy =>
        policy.RequireRole("Admin", "Editor"));

    // ── Política con REQUISITO PERSONALIZADO ─────────────────────────────
    // Usa un handler que valida lógica compleja (edad >= 21)
    opciones.AddPolicy("AtLeast21", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(21)));

    // ── Política que combina ROL + CLAIM + REQUISITO ─────────────────────
    // Solo Admin O (Editor con email verificado Y mayor de 18)
    opciones.AddPolicy("AdminOrVerifiedEditor", policy =>
    {
        policy.RequireAssertion(context =>
        {
            bool isAdmin = context.User.IsInRole("Admin");
            bool isEditor = context.User.IsInRole("Editor");
            bool emailVerified = context.User.HasClaim("email_verified", "true");
            bool isAdult = context.User.HasClaim(c =>
                c.Type == "age" && int.TryParse(c.Value, out int age) && age >= 18);

            return isAdmin || (isEditor && emailVerified && isAdult);
        });
    });
});

// ─── REGISTRAR EL HANDLER PERSONALIZADO ─────────────────────────────────────
// El handler se ejecuta cuando alguien requiere "AtLeast21"
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

WebApplication app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// ─── Base de datos simulada ─────────────────────────────────────────────────
Dictionary<string, UserProfile> users = new()
{
    ["admin"] = new UserProfile("admin", BCrypt.Net.BCrypt.HashPassword("Admin123!"), "Admin", "admin@cms.com", EmailVerified: true, Age: 35),
    ["editor"] = new UserProfile("editor", BCrypt.Net.BCrypt.HashPassword("Editor123!"), "Editor", "editor@cms.com", EmailVerified: true, Age: 25),
    ["author"] = new UserProfile("author", BCrypt.Net.BCrypt.HashPassword("Author123!"), "Author", "author@cms.com", EmailVerified: false, Age: 19),
    ["joven"] = new UserProfile("joven", BCrypt.Net.BCrypt.HashPassword("Joven123!"), "User", "joven@cms.com", EmailVerified: true, Age: 16)
};

// ─── ENDPOINTS ──────────────────────────────────────────────────────────────

// POST /login — Genera token con claims que incluyen rol, email_verified, age
app.MapPost("/login", [AllowAnonymous] ([FromBody] LoginRequest request) =>
{
    if (!users.TryGetValue(request.Username, out UserProfile? user))
        return Results.Unauthorized();

    if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        return Results.Unauthorized();

    SymmetricSecurityKey key = new(keyBytes);
    SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

    // Construimos claims que reflejan todas las propiedades del usuario
    Claim[] claims =
    [
        new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role),                    // Rol para [Authorize(Roles = "...")]
        new Claim("email_verified", user.EmailVerified.ToString().ToLower()), // Claim para policy "RequireEmailVerified"
        new Claim("age", user.Age.ToString()),                    // Claim para policy "AtLeast21"
        new Claim("email", user.Email)
    ];

    System.IdentityModel.Tokens.Jwt.JwtSecurityToken token = new(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
        signingCredentials: creds
    );

    string tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new { access_token = tokenString });
});

// ─── Endpoints clasificados por tipo de protección ──────────────────────────

// Rol simple: solo Admin
app.MapGet("/admin-only", [Authorize(Roles = "Admin")] () =>
    Results.Ok(new { message = "Hola Admin. Solo tú puedes ver esto." }));

// Múltiples roles: Admin o Editor
app.MapGet("/admin-or-editor", [Authorize(Roles = "Admin,Editor")] () =>
    Results.Ok(new { message = "Hola Admin o Editor." }));

// Política basada en claim
app.MapGet("/verified-only", [Authorize(Policy = "RequireEmailVerified")] () =>
    Results.Ok(new { message = "Tu email está verificado." }));

// Política con requisito personalizado (edad >= 21)
app.MapGet("/adults-only", [Authorize(Policy = "AtLeast21")] (ClaimsPrincipal user) =>
    Results.Ok(new { message = $"Tienes {user.FindFirst("age")?.Value} años. Eres mayor de 21." }));

// Política combinada compleja
app.MapGet("/admin-or-verified-editor", [Authorize(Policy = "AdminOrVerifiedEditor")] () =>
    Results.Ok(new { message = "Eres Admin o Editor verificado y adulto." }));

// ─── Autorización imperativa con IAuthorizationService ──────────────────────
// Útil cuando la decisión de autorizar NO se puede tomar con atributos
// (por ejemplo, depende del contenido de un recurso obtenido en el endpoint).
app.MapGet("/resource/{id}", [Authorize] async (
    int id,
    IAuthorizationService authService,
    ClaimsPrincipal user) =>
{
    // Verificar imperativamente si el usuario cumple la política "AtLeast21"
    AuthorizationResult result = await authService.AuthorizeAsync(user, "AtLeast21");

    if (!result.Succeeded)
    {
        return Results.Forbid();
    }

    return Results.Ok(new { resourceId = id, message = $"Acceso concedido al recurso {id}." });
});

// Público
app.MapGet("/public", () => Results.Ok(new { message = "Endpoint público." }));

app.Run();

// ─── MODELOS ─────────────────────────────────────────────────────────────────

public record LoginRequest(string Username, string Password);

public record UserProfile(string Username, string PasswordHash, string Role, string Email, bool EmailVerified, int Age);

// ===================================================================
// REQUISITO Y HANDLER PERSONALIZADO
// ===================================================================

/// <summary>
/// Requisito de autorización: el usuario debe tener al menos cierta edad.
/// Implementa IAuthorizationRequirement (interfaz marcadora).
/// </summary>
/// <param name="MinimumAge">Edad mínima requerida</param>
public class MinimumAgeRequirement(int MinimumAge) : IAuthorizationRequirement
{
    public int MinimumAge { get; } = MinimumAge;
}

/// <summary>
/// Handler que evalúa si el usuario cumple el requisito de edad mínima.
/// Busca el claim "age" en el token y compara con el mínimo requerido.
/// </summary>
public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MinimumAgeRequirement requirement)
    {
        // Buscar el claim "age" en los claims del usuario
        Claim? ageClaim = context.User.FindFirst("age");

        if (ageClaim is not null
            && int.TryParse(ageClaim.Value, out int age)
            && age >= requirement.MinimumAge)
        {
            // El usuario CUMPLE el requisito → autorizado
            context.Succeed(requirement);
        }
        // Si no se llama a context.Succeed(), el requisito falla
        // y la autorización es denegada.

        return Task.CompletedTask;
    }
}
