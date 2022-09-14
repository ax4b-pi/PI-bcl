﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace DuCorp.IdemPotency
{
   public interface IRequestManager
   {
      Task<bool> IsRegistered(
          Guid id,
          CancellationToken cancellationToken = default(CancellationToken));

      Task Register(Guid id,
          CancellationToken cancellationToken = default(CancellationToken));
   }
}
