using MiniECommerce.Api.Common.Requests;
using MiniECommerce.Application.Features.Auth.Commands.Login;

namespace MiniECommerce.Api.Contracts.Auth;

public sealed record LoginRequest(string Email, string Password)
    : ICommandRequest<LoginCommand, LoginResponse>;
