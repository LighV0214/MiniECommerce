using MediatR;

namespace MiniECommerce.Api.Common.Requests;

public interface ICommandRequest<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
{
}
