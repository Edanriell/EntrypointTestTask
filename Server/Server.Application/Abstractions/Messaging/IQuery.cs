using MediatR;
using Server.Domain.Abstractions;

namespace Server.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
  
 
