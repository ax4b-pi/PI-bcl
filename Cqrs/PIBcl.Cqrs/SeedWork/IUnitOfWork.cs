using System;
using System.Threading;
using System.Threading.Tasks;

namespace PIBcl.Cqrs.SeedWork
{
   public interface IUnitOfWork : IDisposable
   {
      Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
      Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));

   }
}
