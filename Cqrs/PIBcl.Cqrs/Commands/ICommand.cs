using MediatR;

namespace PIBcl.Cqrs.Commands
{
   public interface ICommand<out T> : IRequest<T>
   {
   }

   public interface ICommand : ICommand<bool>
   {
   }
}
