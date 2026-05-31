using MediatR;
using MiniECommerce.Application.Features.Auth.Commands.Login;

namespace MiniECommerce.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(string Email, string Password) : IRequest<LoginResponse>;
