using MiniECommerce.Api.Common.Requests;
using MiniECommerce.Application.Features.Auth.Commands.Login;
using MiniECommerce.Application.Features.Auth.Commands.Register;

namespace MiniECommerce.Api.Contracts.Auth;

public sealed record RegisterRequest(string Email, string Password)
    : ICommandRequest<RegisterCommand, LoginResponse>;
