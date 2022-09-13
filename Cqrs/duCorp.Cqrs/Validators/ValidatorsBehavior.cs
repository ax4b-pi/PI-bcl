using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DuCorp.Cqrs.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace DuCorp.Cqrs.Validators
{
   public class ValidatorsBehavior<TRequest, TResponse>
      : IPipelineBehavior<TRequest, TResponse>
      where TRequest : IRequest<TResponse> where TResponse : Response
   {
      private readonly IValidator<TRequest>[] _validators;

      public ValidatorsBehavior(IValidator<TRequest>[] validators)
      {
         _validators = validators;
      }

      public Task<TResponse> Handle(
          TRequest request,
          CancellationToken cancellationToken,
          RequestHandlerDelegate<TResponse> next)
      {
         var failures = _validators
             .Select(validator => validator.Validate(request))
             .SelectMany(result => result.Errors)
             .Where(error => error != null)
             .ToList();

         return failures.Any()
               ? Errors(failures)
               : next();
      }

      private static Task<TResponse> Errors(IEnumerable<ValidationFailure> failures)
      {
         var response = new Response();

         foreach (var failure in failures)
         {
            response.AddError(failure.ErrorMessage);
         }

         return Task.FromResult(response as TResponse);
      }
   }
}
