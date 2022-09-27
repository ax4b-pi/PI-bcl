using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using PIBcl.IdemPotency;
using PIBcl.Cqrs.Exceptions;

namespace PIBcl.Cqrs.Commands
{
   public class IdentifiedCommandHandler<TCommand, TResult>
          : IRequestHandler<IdentifiedCommand<TCommand, TResult>, TResult>
          where TCommand : IRequest<TResult>
   {
      private readonly IMediator _mediator;
      private readonly IRequestManager _requestManager;

      public IdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager)
      {
         _mediator = mediator;
         _requestManager = requestManager;
      }

      public async Task<TResult> Handle(
          IdentifiedCommand<TCommand, TResult> message,
          CancellationToken cancellationToken = default(CancellationToken)
          )
      {
         if (message.Id == Guid.Empty)
         {
            ThrowPipelineException.IdentifiedCommandWithoutId();
         }

         if (message.Command == null)
         {
            ThrowPipelineException.IdentifiedCommandWithoutInnerCommand();
         }

         var alreadyRegistered = await _requestManager.IsRegistered(message.Id, cancellationToken);
         if (alreadyRegistered)
         {
            ThrowPipelineException.CommandWasAlreadyExecuted();
         }

         await _requestManager.Register(message.Id, cancellationToken);
         var result = await _mediator.Send(message.Command, cancellationToken);
         return result;
      }
   }
}
