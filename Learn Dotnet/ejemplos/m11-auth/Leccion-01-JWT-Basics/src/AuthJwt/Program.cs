// ===================================================================
// Lección 01 — JWT Basics
// ===================================================================
// ¿Qué es JWT?
//   JSON Web Token es un estándar abierto (RFC 7519) para transmitir
//   información segura entre partes como un JSON firmado.
//
//   Estructura: Header.Payload.Signature (todo en Base64)
//
//   Header:   { "alg": "HS256", "typ": "JWT" }
//             Indica el algoritmo HMAC-SHA256 para firmar.
//
//   Payload:  { "sub": "usuario1", "name": "Juan", "role": "Admin", "exp": 1710000000 }
//             Contiene los claims (afirmaciones) sobre el usuario.
//             exp = expiration time (tiempo de expiración en segundos Unix).
//
//   Signature: HMAC-SHA256( Base64(Header) + "." + Base64(Payload), secretKey )
//             Garantiza que nadie ha modificado el token
//             porque solo el servidor conoce la clave secreta.
//
// Flujo de autenticación JWT:
//   1. Cliente envía usuario/contraseña → POST /login
//   2. Servidor valida credenciales (BCrypt) y genera JWT
//   3. Cliente guarda el token y lo envía en cada request:
//      Authorization: Bearer <token>
//   4. Servidor valida firma + expiración sin consultar BD
//
// Por qué JWT es stateless:
//   - El token contiene toda la información del usuario en su payload.
//   - El servidor no necesita almacenar sesiones en memoria/BD.
//   - Cada servidor puede validar el token con la misma clave secreta.
//   - Esto permite escalar horizontalmente sin compartir estado.
// ===================================================================

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ─── CONFIGURACIÓN ───────────────────────────────────────────────────────────
// Leemos la configuración de JWT desde appsettings.json
string secretKey = builder.Configuration["JwtSettings:SecretKey"]!;
string issuer = builder.Configuration["JwtSettings:Issuer"]!;
string audience = builder.Configuration["JwtSettings:Audience"]!;
int expirationMinutes = int.Parse(builder.Configuration["JwtSettings:ExpirationMinutes"]!);

// ─── AUTENTICACIÓN JWT ──────────────────────────────────────────────────────
// Configuramos el middleware de autenticación JWT.
// Esto es lo que hace que ASP.NET Core valide automáticamente
// los tokens que llegan en el header Authorization: Bearer <token>
builder.Services
    .AddAuthentication("Bearer") // Esquema por defecto: "Bearer"
    .AddJwtBearer(opciones =>
    {
        // Parámetros que se usan para validar cada token entrante
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            // Valida que el emisor (iss) del token coincida con el nuestro
            ValidateIssuer = true,
            ValidIssuer = issuer,

            // Valida que la audiencia (aud) del token coincida
            ValidateAudience = true,
            ValidAudience = audience,

            // Valida que el token no haya expirado (exp)
            ValidateLifetime = true,

            // Valida la firma HMAC-SHA256 con nuestra clave secreta
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)
            ),

            // Margen de tolerancia para la expiración (0 = exacto)
            ClockSkew = TimeSpan.Zero
        };
    });

// ─── AUTORIZACIÓN ────────────────────────────────────────────────────────────
// Registramos el middleware de autorización (necesario para usar [Authorize])
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

app.UseAuthentication(); // Middleware de autenticación (va antes de Authorization)
app.UseAuthorization();  // Middleware de autorización

// ─── Base de datos simulada (diccionario en memoria) ─────────────────────────
// En producción usaríamos Entity Framework con PostgreSQL.
// Aquí usamos BCrypt para hashear las contraseñas.
Dictionary<string, User> usersDb = new()
{
    ["admin"] = new User("admin", BCrypt.Net.BCrypt.HashPassword("Admin123!"), "Administrador", "Admin"),
    ["editor"] = new User("editor", BCrypt.Net.BCrypt.HashPassword("Editor123!"), "Editor", "Editor"),
    ["author"] = new User("author", BCrypt.Net.BCrypt.HashPassword("Author123!"), "Autor", "Author")
};

// ─── ENDPOINTS ──────────────────────────────────────────────────────────────

// POST /login — Endpoint público (sin autenticación)
// Recibe usuario/contraseña, valida y devuelve el token JWT
app.MapPost("/login", [AllowAnonymous] ([FromBody] LoginRequest request) =>
{
    // Buscar usuario en nuestra "base de datos" simulada
    if (!usersDb.TryGetValue(request.Username, out User? user))
    {
        return Results.Unauthorized();
    }

    // Verificar contraseña con BCrypt (compara hash, nunca texto plano)
    if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    {
        return Results.Unauthorized();
    }

    // ─── GENERACIÓN DEL TOKEN JWT ───────────────────────────────────────
    // 1. Creamos la clave simétrica a partir de nuestra clave secreta
    SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));

    // 2. Definimos las credenciales de firma (algoritmo + clave)
    SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

    // 3. Construimos los claims que irán en el payload del token
    Claim[] claims =
    [
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),     // subject = identificador del usuario
        new Claim(JwtRegisteredClaimNames.Name, user.FullName),   // nombre completo
        new Claim(ClaimTypes.Role, user.Role),                    // rol (para [Authorize(Roles = "...")])
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID único del token
    ];

    // 4. Creamos el token JWT
    JwtSecurityToken token = new(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
        signingCredentials: credentials
    );

    // 5. Serializamos el token a string
    string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    // Devolvemos el token al cliente
    return Results.Ok(new { access_token = tokenString, token_type = "Bearer", expires_in = expirationMinutes * 60 });
});

// GET /me — Endpoint protegido
// Solo accesible si el request incluye un token JWT válido
// [Authorize] = requiere estar autenticado
app.MapGet("/me", [Authorize] (ClaimsPrincipal user) =>
{
    // Extraemos los claims del token validado.
    // ASP.NET ya hizo todo el trabajo de validación en el middleware.
    List<object> claims = user.Claims.Select(c => new
    {
        c.Type,
        c.Value
    }).ToList<object>();

    return Results.Ok(new
    {
        username = user.FindFirst(ClaimTypes.Name)?.Value,
        role = user.FindFirst(ClaimTypes.Role)?.Value,
        claims
    });
});

// GET /public — Endpoint público (sin autenticación)
app.MapGet("/public", () => Results.Ok(new { message = "Este endpoint es público. No requiere token." }));

// GET /admin — Endpoint que requiere rol Admin
app.MapGet("/admin", [Authorize(Roles = "Admin")] () =>
    Results.Ok(new { message = "Solo los administradores pueden ver esto." }));

app.Run();

// ─── MODELOS ─────────────────────────────────────────────────────────────────
/// <summary>
/// Request de login: usuario y contraseña en texto plano.
/// La contraseña viaja en el body del POST.
/// </summary>
public record LoginRequest(string Username, string Password);

/// <summary>
/// Usuario simulado. En producción, esto sería una entidad de Entity Framework.
/// La contraseña NUNCA se almacena en texto plano, siempre hasheada con BCrypt.
/// </summary>
/// <param name="Username">Nombre de usuario único</param>
/// <param name="PasswordHash">Hash BCrypt de la contraseña</param>
/// <param name="FullName">Nombre completo para mostrar</param>
/// <param name="Role">Rol del usuario (Admin, Editor, Author)</param>
public record User(string Username, string PasswordHash, string FullName, string Role);
