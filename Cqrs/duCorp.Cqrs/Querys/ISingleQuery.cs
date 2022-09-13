using MediatR;


namespace DuCorp.Cqrs.Querys
{
   public interface ISingleQuery<out T> : IRequest<T>
   {
   }

   public interface ISingleQuery : ISingleQuery<bool>
   {
   }
}
