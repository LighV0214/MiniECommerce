using System.Security.Claims;
using MiniECommerce.Domain.Entities;

namespace MiniECommerce.Application.Common.Interfaces;

public interface IJwtService
{
    JwtToken GenerateToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}

public sealed record JwtToken(string AccessToken, DateTimeOffset ExpiresAt);
