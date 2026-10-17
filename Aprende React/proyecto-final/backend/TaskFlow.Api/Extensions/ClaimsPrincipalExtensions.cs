using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace TaskFlow.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var userId))
            throw new UnauthorizedAccessException("Usuario no autenticado");

        return userId;
    }
}
