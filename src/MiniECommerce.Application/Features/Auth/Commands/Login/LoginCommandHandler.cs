using MediatR;
using MiniECommerce.Application.Common.Exceptions;
using MiniECommerce.Application.Common.Interfaces;
using MiniECommerce.Domain.Entities;

namespace MiniECommerce.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtService jwtService)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(request.Email);
        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var token = jwtService.GenerateToken(user);

        return new LoginResponse(
            user.Id,
            user.Email,
            user.Role.ToString(),
            token.AccessToken,
            token.ExpiresAt);
    }
}
