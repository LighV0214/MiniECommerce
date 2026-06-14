using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Api.Contracts.Auth;
using MiniECommerce.Application.Features.Auth.Commands.Login;

namespace MiniECommerce.Api.Controllers;

[Route("api/auth")]
public sealed class AuthController(ISender sender) : ApiControllerBase(sender)
{
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<ActionResult<LoginResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        return DispatchCommandAsync(request, cancellationToken);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        return DispatchCommandAsync(request, cancellationToken);
    }
}
