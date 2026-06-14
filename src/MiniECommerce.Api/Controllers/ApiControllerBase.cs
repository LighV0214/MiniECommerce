using MediatR;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.Api.Common.Mapping;
using MiniECommerce.Api.Common.Requests;

namespace MiniECommerce.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase(ISender sender) : ControllerBase
{
    protected async Task<ActionResult<TResponse>> DispatchCommandAsync<TCommand, TResponse>(
        ICommandRequest<TCommand, TResponse> request,
        CancellationToken cancellationToken)
        where TCommand : IRequest<TResponse>
    {
        var command = CommandRequestMapper.Map<TCommand>(request);
        var response = await sender.Send(command, cancellationToken);

        return Ok(response);
    }
}
