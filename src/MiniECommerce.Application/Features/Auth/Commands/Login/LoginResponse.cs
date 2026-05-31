namespace MiniECommerce.Application.Features.Auth.Commands.Login;

public sealed record LoginResponse(
    Guid UserId,
    string Email,
    string Role,
    string AccessToken,
    DateTimeOffset ExpiresAt);
