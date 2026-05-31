using MediatR;
using MiniECommerce.Application.Common.Exceptions;
using MiniECommerce.Application.Common.Interfaces;
using MiniECommerce.Application.Features.Auth.Commands.Login;
using MiniECommerce.Domain.Entities;

namespace MiniECommerce.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtService jwtService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(request.Email);

        if (await userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken))
        {
            throw new ConflictException("A user with this email already exists.");
        }

        var user = new User(
            normalizedEmail,
            passwordHasher.HashPassword(request.Password));

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = jwtService.GenerateToken(user);

        return new LoginResponse(
            user.Id,
            user.Email,
            user.Role.ToString(),
            token.AccessToken,
            token.ExpiresAt);
    }
}
